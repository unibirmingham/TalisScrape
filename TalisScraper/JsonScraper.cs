using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TalisScraper.Objects;
using Cache;
using Extensions;
using Newtonsoft.Json.Linq;
using NLog;
using TalisScraper.Enums;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;
using TalisScraper.Objects.JsonMaps;

//Make internals visible to testing framework
#if DEBUG
[assembly: InternalsVisibleTo("TalisScrapeTest.Tests")]
#endif
namespace TalisScraper
{
    /// <summary>
    /// Scraper to fetch and parse Talis Aspire json reading list information
    /// </summary>
    public class JsonScraper : IScraper
    {
        #region Private Vars
        private const string RootRegex = "\"([^\"]+)\"";
        private readonly IRequestHandler _requestHandler;
        private bool _scrapeInProgress = false;

        private readonly ScrapeConfig _scrapeConfig;

        private volatile bool _scrapeCancelled;

        private Stopwatch _stopwatch;

        //todo: Have a collection of reports stored from current app start? Perhaps each scrape has a guid or UID, and report stored against that? Might help for DB storage.
        private ScrapeReport _scrapeReport = null;
        #endregion

        public JsonScraper(IRequestHandler requestHandler, ScrapeConfig scrapeConfig = null)
        {
            Log = LogManager.GetCurrentClassLogger();//todo: inject this in?
            _requestHandler = requestHandler;
            _scrapeConfig = scrapeConfig;

            _stopwatch = new Stopwatch();
        }

        public ILogger Log { get; set; }
        public ICache Cache { get; set; }
        public IExportHandler ExportHandler { get; set; } 
        
        #region Event Definitions
        public event EventHandler<ScrapeEndedEventArgs> ScrapeEnded;
        public event EventHandler<ScrapeCancelledEventArgs> ScrapeCancelled;
        public event EventHandler<ScrapeFailedEventArgs> ScrapeFailed;
        public event EventHandler<ScrapeStartedEventArgs> ScrapeStarted;
        public event EventHandler<ResourceScrapedEventArgs> ResourceScraped;
        #endregion

        #region Async Functions
        /// <summary>
        /// Fetches json object from the specified uri using async
        /// </summary>
        /// <param name="uri">uri of json object</param>
        /// <returns>a string json object</returns>
        internal async Task<string> FetchJsonAsync(string uri)
        {
            var json = await _requestHandler.FetchJsonAsync(uri);

            if (string.IsNullOrEmpty(json) && _scrapeReport != null)
                _scrapeReport.FailedScrapes.Add(uri);

            if (_scrapeReport != null)
                _scrapeReport.TotalRequestsMade++;

            if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri));

            return json;
        }

        internal async Task<NavItem> FetchItemsInternalAsync(string uri)
        {
            var basObj = Cache.FetchItem<NavItem>(uri);

            if (basObj == null)
            {

                var json = await FetchJsonAsync(uri);


                basObj = NavItemParser(json);

                if (basObj != null)
                {
                    Cache.PutItem(basObj, uri);
                }
            }
            else
            {
                if (_scrapeReport != null)
                    _scrapeReport.TotalCacheRequestsMade++;
            }

            return basObj;
        }

        public async Task<NavItem> FetchNavItemAsync(string uri)
        {
            if (_scrapeInProgress)
                return null;

            _scrapeInProgress = true;
            _scrapeCancelled = false;

            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));

            var items = await FetchItemsInternalAsync(uri);

            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            _scrapeInProgress = false;

            return items;
        }

        private async Task RecParseAsync(string loc, List<string> list)
        {
            if (_scrapeCancelled)
                return;

           // await Task.Yield();
            var items = await FetchItemsInternalAsync(loc);

            if (items != null)
            {
                foreach (var ou in items.Items.OrganizationalUnit ?? new Element[] {})
                {
                    await RecParseAsync(string.Format("{0}.json", ou.Value), list);
                }

                foreach (var ou in items.Items.KnowledgeGrouping ?? new Element[] { })
                {
                    await RecParseAsync(string.Format("{0}.json", ou.Value), list);
                }

                if (items.Items.UsesList.HasContent())
                {
                    list.AddRange(items.Items.UsesList.Select(n => n.Value));
                }
            }
        }

        private async Task RecParseParallelAsync(string loc, ConcurrentBag<string> list)
        {
            if (_scrapeCancelled)
                return;

            var items = await FetchItemsInternalAsync(loc);

            if (items != null)
            {
                if (items.Items.OrganizationalUnit.HasContent())
                {
                    Parallel.ForEach(items.Items.OrganizationalUnit, async (item, state) =>
                    {
                        await RecParseParallelAsync(string.Format("{0}.json", item.Value), list);
                    });
                }

                if (items.Items.KnowledgeGrouping.HasContent())
                {
                    Parallel.ForEach(items.Items.KnowledgeGrouping, async (item, state) =>
                    {
                        await RecParseParallelAsync(string.Format("{0}.json", item.Value), list);
                    });
                }

                if (items.Items.UsesList.HasContent())
                {
                    Parallel.ForEach(items.Items.UsesList, (item, state) =>
                    {
                        list.Add(item.Value);
                    });
                }
            }
        }

        public async Task<IEnumerable<string>> ScrapeReadingListsAsync(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Log.Error("Could not initiate scrape. The root node address was empty.");
                return null;
            }

            const ScrapeType scrapeType = ScrapeType.ReadingList;

            if (!ScrapeInitiation(scrapeType))
                return null;

            var lists = new List<string>();

            if (_scrapeConfig != null && _scrapeConfig.EnableParallelProcessing)
            {
                var listsP = new ConcurrentBag<string>();

                await RecParseParallelAsync(root, listsP);

                lists = listsP.ToList();
            }
            else
            {
                await RecParseAsync(root, lists);
            }

            ScrapeCleanup(scrapeType);

            return lists;
        }

        public async Task<IEnumerable<ReadingList>> PopulateReadingListsAsync(IEnumerable<string> readingLists)
        {
            if (!readingLists.HasContent())
            {
                Log.Error("Attempted to populate reading lists, but passed in list object was null.");
                return null;
            }

            const ScrapeType scrapeType = ScrapeType.Books;

            if (!ScrapeInitiation(scrapeType))
                return null;

            IEnumerable<ReadingList> readingListsFinal;

            if (_scrapeConfig != null && _scrapeConfig.EnableParallelProcessing)
                readingListsFinal = await DoPopulateReadingListsParallelAsync(readingLists);
            else
                readingListsFinal = await DoPopulateReadingListsAsync(readingLists);

            ScrapeCleanup(scrapeType);

            return readingListsFinal;
        }

        private async Task<IEnumerable<ReadingList>> DoPopulateReadingListsAsync(IEnumerable<string> readingLists)
        {
            var readingListCollection = new Collection<ReadingList>();
            foreach (var item in readingLists)
            {
                var uri = string.Format("{0}.json", item);

                var readingListNavObj = await FetchItemsInternalAsync(uri);

                if (readingListNavObj == null)
                {
                    Log.Error("Reading list from uri:{0} could not be scraped.", uri);
                    continue;
                }

                readingListCollection.Add(new ReadingList { Uri = uri, ListInfo = readingListNavObj });

            }

            if (readingListCollection.HasContent())
            {//fetch books from discovered lists and add them to the relevant list
                foreach (var rlItem in readingListCollection)
                {
                    if (_scrapeCancelled) return null;
                    foreach (var rlItemList in rlItem.ListInfo.Items.Contains)
                    {
                        if (_scrapeCancelled) return null;
                        if (rlItemList.Value.Contains("/items/"))
                        {
                            var getbookItems = FetchItemsInternal(rlItem.Uri);

                            if (getbookItems != null && getbookItems.Items.Contains.HasContent())
                            {//scrape individual book info
                                foreach (var book in getbookItems.Items.Contains)
                                {
                                    if (_scrapeCancelled) return null;
                                    if (book.Value.Contains("/items/"))
                                    {
                                        var bookItem = FetchJson(string.Format("{0}.json", book.Value));

                                        if (!string.IsNullOrEmpty(bookItem))
                                        {
                                            var bookObj = ParseBookInfoFromJson(book.Value, bookItem);

                                            if (bookObj != null)
                                                rlItem.Books.Add(bookObj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return readingListCollection;
        }

        private async Task<IEnumerable<ReadingList>> DoPopulateReadingListsParallelAsync(IEnumerable<string> readingLists)
        {
            var readingListCollection = new ConcurrentBag<ReadingList>();

            Parallel.ForEach(readingLists, async (currentUri, state) =>
            {
                if (_scrapeCancelled) state.Break();
                var uri = string.Format("{0}.json", currentUri);

                var readingListNavObj = await FetchItemsInternalAsync(uri);

                if (readingListNavObj == null)
                {
                    Log.Error("Reading list from uri:{0} could not be scraped.", uri);

                }
                else
                {
                    readingListCollection.Add(new ReadingList { Uri = uri, ListInfo = readingListNavObj });
                }


            });

            var tst = readingListCollection;//.ToArray();

            if (tst.HasContent())
            {//fetch books from discovered lists and add them to the relevant list

                Parallel.ForEach(readingListCollection, async (currentList, state) =>
                {
                    if (_scrapeCancelled) state.Break();
                    foreach (var rlItemList in currentList.ListInfo.Items.Contains)
                    {
                        if (_scrapeCancelled) state.Break();
                        if (rlItemList.Value.Contains("/items/"))
                        {
                            var getbookItems = await FetchItemsInternalAsync(currentList.Uri);

                            if (getbookItems != null && getbookItems.Items.Contains.HasContent())
                            {//scrape individual book info

                                Parallel.ForEach(getbookItems.Items.Contains, (item, subState) =>
                                {
                                    {
                                        if (_scrapeCancelled) state.Break();
                                        if (item.Value.Contains("/items/"))
                                        {
                                            var bookItem = FetchJson(string.Format("{0}.json", item.Value));

                                            if (!string.IsNullOrEmpty(bookItem))
                                            {
                                                var bookObj = ParseBookInfoFromJson(item.Value, bookItem);

                                                if (bookObj != null)
                                                    currentList.Books.Add(bookObj);
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }


                });


            }

            return readingListCollection.ToList();
        }

        #endregion

        #region Sync Functions
        /// <summary>
        /// Fetches json object from the specified uri
        /// </summary>
        /// <param name="uri">uri of json object</param>
        /// <returns>a string json object</returns>
        internal string FetchJson(string uri)
        {
            var json = _requestHandler.FetchJson(uri);

            if (string.IsNullOrEmpty(json) && _scrapeReport != null)
                _scrapeReport.FailedScrapes.Add(uri);

            if (_scrapeReport != null)
                _scrapeReport.TotalRequestsMade++;

            if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri));

            return json;
        }

        private NavItem NavItemParser(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var replaceRootRegex = new Regex(RootRegex);

            var finalJson = replaceRootRegex.Replace(json, "\"root\"", 1);
            NavItem convertedNav = null;

            try
            {
                convertedNav = JsonConvert.DeserializeObject<NavItem>(finalJson);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return convertedNav;
        }

        /// <summary>
        /// Fetches Items, has been made internal so the public Fetch Item func can fire scrape start and stop events for individual items
        /// </summary>
        internal NavItem FetchItemsInternal(string uri)
        {
            var basObj = Cache.FetchItem<NavItem>(uri);

            if (basObj == null)
            {
                var json = FetchJson(uri);

                basObj = NavItemParser(json);

                if (basObj != null)
                {
                    Cache.PutItem(basObj, uri);
                }

            }
            else
            {
                if (_scrapeReport != null)
                    _scrapeReport.TotalCacheRequestsMade++;
            }

            return basObj;
        }

        public NavItem FetchNavItem(string uri)
        {
            const ScrapeType scrapeType = ScrapeType.Item;

            if (!ScrapeInitiation(scrapeType))
                return null;

            var items = FetchItemsInternal(uri);

            ScrapeCleanup(scrapeType);

            return items;
        }

        private void RecParse(string loc, ref List<string> list)
        {
            if (_scrapeCancelled)
                return;

            var items = FetchItemsInternal(loc);

            if (items != null)
            {
                foreach (var ou in items.Items.OrganizationalUnit ?? new Element[] { })
                {
                    RecParse(string.Format("{0}.json", ou.Value), ref list);
                }

                foreach (var ou in items.Items.KnowledgeGrouping ?? new Element[] { })
                {
                    RecParse(string.Format("{0}.json", ou.Value), ref list);
                }

                if (items.Items.UsesList.HasContent())
                {
                    list.AddRange(items.Items.UsesList.Select(n =>  n.Value));
                }
            }

        }

        private void RecParseParallel(string loc, ConcurrentBag<string> list)
        {
            if (_scrapeCancelled)
                return;

            var items = FetchItemsInternal(loc);

            if (items != null)
            {
                if (items.Items.OrganizationalUnit.HasContent())
                {
                    Parallel.ForEach(items.Items.OrganizationalUnit, (item, status) =>
                    {
                        RecParseParallel(string.Format("{0}.json", item.Value), list);
                    });
                }

                if (items.Items.KnowledgeGrouping.HasContent())
                {
                    Parallel.ForEach(items.Items.KnowledgeGrouping, (item, status) =>
                    {
                        RecParseParallel(string.Format("{0}.json", item.Value), list);
                    });
                }

                if (items.Items.UsesList.HasContent())
                {
                    Parallel.ForEach(items.Items.UsesList, (item, status) =>
                    {
                        list.Add(item.Value);
                    });
                    
                }
            }

        }

        public IEnumerable<string> ScrapeReadingLists(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Log.Fatal("Scraper.ParseTest: Could not initiate scrape. The root node address was empty.");
                return null;
            }

            const ScrapeType scrapeType = ScrapeType.ReadingList;

            if (!ScrapeInitiation(scrapeType))
                return null;

            var lists = new List<string>();


            if (_scrapeConfig != null && _scrapeConfig.EnableParallelProcessing)
            {
                var listP = new ConcurrentBag<string>();
                RecParseParallel(root, listP);

                lists = listP.ToList();
            }
            else
            {
                RecParse(root, ref lists);
            }

            ScrapeCleanup(scrapeType);

            return lists;
        }

        private IEnumerable<ReadingList> DoPopulateReadingListsParallel(IEnumerable<string> readingLists)
        {
            var readingListCollection = new ConcurrentBag<ReadingList>();

            Parallel.ForEach(readingLists, (currentUri, state) =>
            {
                if (_scrapeCancelled) state.Break();
                var uri = string.Format("{0}.json", currentUri);

                var readingListNavObj = FetchItemsInternal(uri);

                if (readingListNavObj == null)
                {
                    Log.Error("Reading list from uri:{0} could not be scraped.", uri);

                }
                else
                {
                    readingListCollection.Add(new ReadingList { Uri = uri, ListInfo = readingListNavObj });
                }

                
            });

            var tst = readingListCollection;//.ToArray();

            if (tst.HasContent())
            {//fetch books from discovered lists and add them to the relevant list

                Parallel.ForEach(readingListCollection, (currentList, state) =>
                {
                    if (_scrapeCancelled) state.Break();
                    foreach (var rlItemList in currentList.ListInfo.Items.Contains)
                    {
                        if (_scrapeCancelled) state.Break();
                        if (rlItemList.Value.Contains("/items/"))
                        {
                            var getbookItems = FetchItemsInternal(currentList.Uri);

                            if (getbookItems != null && getbookItems.Items.Contains.HasContent())
                            {//scrape individual book info

                                Parallel.ForEach(getbookItems.Items.Contains, (item, subState) =>
                                {
                                    {
                                        if (_scrapeCancelled) state.Break();
                                        if (item.Value.Contains("/items/"))
                                        {
                                            var bookItem = FetchJson(string.Format("{0}.json", item.Value));

                                            if (!string.IsNullOrEmpty(bookItem))
                                            {
                                                var bookObj = ParseBookInfoFromJson(item.Value, bookItem);

                                                if (bookObj != null)
                                                    currentList.Books.Add(bookObj);
                                            }
                                        }
                                    }
                                });
                            }
                        }
                    }


                });


            }

            return readingListCollection.ToList();
        }

        private IEnumerable<ReadingList> DoPopulateReadingLists(IEnumerable<string> readingLists)
        {
            var readingListCollection = new Collection<ReadingList>();
            foreach (var item in readingLists)
            {
                var uri = string.Format("{0}.json", item);

                var readingListNavObj = FetchItemsInternal(uri);

                if (readingListNavObj == null)
                {
                    Log.Error("Reading list from uri:{0} could not be scraped.", uri);
                    continue;
                }

                readingListCollection.Add(new ReadingList { Uri = uri, ListInfo = readingListNavObj });

            }

            if (readingListCollection.HasContent())
            {//fetch books from discovered lists and add them to the relevant list
                foreach (var rlItem in readingListCollection)
                {
                    if (_scrapeCancelled) return null;
                    foreach (var rlItemList in rlItem.ListInfo.Items.Contains)
                    {
                        if (_scrapeCancelled) return null;
                        if (rlItemList.Value.Contains("/items/"))
                        {
                            var getbookItems = FetchItemsInternal(rlItem.Uri);

                            if (getbookItems != null && getbookItems.Items.Contains.HasContent())
                            {//scrape individual book info
                                foreach (var book in getbookItems.Items.Contains)
                                {
                                    if (_scrapeCancelled) return null;
                                    if (book.Value.Contains("/items/"))
                                    {
                                        var bookItem = FetchJson(string.Format("{0}.json", book.Value));

                                        if (!string.IsNullOrEmpty(bookItem))
                                        {
                                            var bookObj = ParseBookInfoFromJson(book.Value, bookItem);

                                            if (bookObj != null)
                                                rlItem.Books.Add(bookObj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return readingListCollection;
        }

        //todo: how does cancel scrape fit into this? Might pass a prescraped collection in, so can't assume we outright cancel it
        public IEnumerable<ReadingList> PopulateReadingLists(IEnumerable<string> readingLists)
        {//scrape lists from passed in uri collection of lists

            if (!readingLists.HasContent())
            {
                Log.Error("Attempted to populate reading lists, but passed in list object was null.");
                return null;
            }

            const ScrapeType scrapeType = ScrapeType.Books;

            if (!ScrapeInitiation(scrapeType))
                return null;

            IEnumerable<ReadingList> readingListsFinal;

            if (_scrapeConfig != null && _scrapeConfig.EnableParallelProcessing)
                readingListsFinal = DoPopulateReadingListsParallel(readingLists);
            else
                readingListsFinal = DoPopulateReadingLists(readingLists);

            ScrapeCleanup(scrapeType);

            return readingListsFinal;
        }
        #endregion

        public string DoExport(IEnumerable<ReadingList> readingLists)
        {
            if (ExportHandler == null)
            {
                Log.Error("ExportHandler is not configured.");
                return string.Empty;
            }

            return ExportHandler.Export(readingLists);
        }

        #region shared functions

        /// <summary>
        /// Initiates vars and events prior to starting a scrape
        /// </summary>
        /// <returns></returns>
        private bool ScrapeInitiation(ScrapeType type)
        {
            if (_scrapeInProgress)
                return false;

            _scrapeInProgress = true;

            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(type));
            _scrapeReport = new ScrapeReport { ScrapeStarted = DateTime.Now };

            _stopwatch = new Stopwatch();
            _stopwatch.Start();

            _scrapeCancelled = false;



            return true;
        }

        /// <summary>
        /// resets vars and cleans up events after a scrape has finished
        /// </summary>
        private void ScrapeCleanup(ScrapeType type)
        {
            _stopwatch.Stop();

            _scrapeReport.ScrapeEnded = DateTime.Now;
            _scrapeReport.TimeTaken = _stopwatch.Elapsed;

            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(type));


            _scrapeInProgress = false;
        }

        public bool CancelScrape()
        {
            _scrapeCancelled = true;

            if(ScrapeCancelled != null) ScrapeCancelled(this, new ScrapeCancelledEventArgs());

            return true;
        }
        private Book ParseBookInfoFromJson(string uri, string json)
        {
            var book = new Book();

            try
            {

                var jObj = JObject.Parse(json);


                var rawOrg = jObj.Properties().Where(p => p.Name.Contains("/organisations/"));
                var rawResources = jObj.Properties().Where(p => p.Name.Contains("/resources/"));// && !p.Name.Contains("/authors"));
                var rawPeople = jObj.Properties().Where(p => p.Name.Contains("/people/"));

                if (rawOrg.HasContent())
                {
                    //the organisation json nodes contains publisher information
                    foreach (var org in rawOrg)
                    {
                        foreach (var child in org.Children())
                        {
                            if (child.HasValues && child["http://xmlns.com/foaf/0.1/name"] != null)
                            {
                                var childa =
                                    JsonConvert.DeserializeObject<Element>(
                                        child["http://xmlns.com/foaf/0.1/name"][0].ToString());

                                book.Publisher += string.Join(", ", childa != null ? childa.Value : string.Empty);
                            }
                        }
                    }
                }

                if (rawPeople.HasContent())
                {
                    //the organisation json nodes contains publisher information
                    foreach (var person in rawPeople)
                    {
                        foreach (var child in person.Children())
                        {
                            if (child.HasValues && child["http://xmlns.com/foaf/0.1/name"] != null)
                            {
                                var childa =
                                    JsonConvert.DeserializeObject<Element>(
                                        child["http://xmlns.com/foaf/0.1/name"][0].ToString());

                                if (childa != null)
                                    book.Authors.Add(childa.Value);
                            }
                        }
                    }
                }

                //the resources json nodes contain dteails about the book/journal
                if (rawResources.HasContent())
                {
                    foreach (var resource in rawResources)
                    {
                        var resJson = resource.ToString();

                        var replaceRootRegex = new Regex(RootRegex);

                        var finalJson = replaceRootRegex.Replace(resJson, "\"root\"", 1);

                        var resourceObj = JsonConvert.DeserializeObject<Resources>("{" + finalJson + "}");

                        if (resourceObj != null && resourceObj.Items != null)
                        {
                            if (resourceObj.Items.Title.HasContent())
                                book.Title += string.Join(", ", resourceObj.Items.Title.Select(n => n.Value));

                            if (resourceObj.Items.Date.HasContent())
                                book.Date.AddRange(resourceObj.Items.Date.Select(n => n.Value));// = resourceObj.Items.Date.FirstOrDefault().Value; //todo: should we check if there are multiple dates?

                            if (resourceObj.Items.Subject.HasContent())
                                book.Subject.AddRange(resourceObj.Items.Subject.Select(n => n.Value));

                            if (resourceObj.Items.Isbn10.HasContent())
                                book.Isbn10.AddRange(resourceObj.Items.Isbn10.Select(n => n.Value));

                            if (resourceObj.Items.Isbn13.HasContent())
                                book.Isbn13.AddRange(resourceObj.Items.Isbn13.Select(n => n.Value));

                            if (resourceObj.Items.Source.HasContent())
                                book.Source.AddRange(resourceObj.Items.Source.Select(n => n.Value));

                            if (resourceObj.Items.PlaceOfPublication.HasContent())
                                book.PlaceOfPublication += string.Join(", ", resourceObj.Items.PlaceOfPublication.Select(n => n.Value));

                            book.Url = uri;
                        }
                    }

                }



            }
            catch (Exception ex)
            {
                Log.Error(ex);

                book = null;
            }


            return book;
        }
        public ScrapeReport FetchScrapeReport()
        {
            return _scrapeReport;
        }
        #endregion
    }
}
