using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
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

//TODO: should we lock per scrape, so if another scrape is initiated before current scrap[e ends, we deny it? Also a 'stop scrape' function?

//Make internals visible to testing framework
#if DEBUG
[assembly: InternalsVisibleTo("TalisScrapeTest.Tests")]
#endif
namespace TalisScraper
{
    //TODO: have a scrape options class for configuration?
    public class JsonScraper : IScraper
    {
        private const string RootRegex = "\"([^\"]+)\"";
        private readonly IRequestHandler _requestHandler;

        private volatile bool _scrapeCancelled;

        private ScrapeReport _scrapeReport = null;

        public JsonScraper(IRequestHandler requestHandler)
        {
  

            Log = LogManager.GetCurrentClassLogger();//todo: inject this in?
            _requestHandler = requestHandler;

            
        }

        public ILogger Log { get; set; }
        public ICache Cache { get; set; }
        
        #region Async Functions
        public event EventHandler<ScrapeEndedEventArgs> ScrapeEnded;
        public event EventHandler<ScrapeCancelledEventArgs> ScrapeCancelled;
        public event EventHandler<ScrapeStartedEventArgs> ScrapeStarted;
        public event EventHandler<ResourceScrapedEventArgs> ResourceScraped;

        /// <summary>
        /// Fetches json object from the specified uri using async
        /// </summary>
        /// <param name="uri">uri of json object</param>
        /// <returns>a string json object</returns>
        internal async Task<string> FetchJsonAsync(string uri)
        {
            var json = await _requestHandler.FetchJsonAsync(uri).ConfigureAwait(false);

            if (string.IsNullOrEmpty(json) && _scrapeReport != null)
                _scrapeReport.FailedScrapes.Add(uri);

            if (_scrapeReport != null)
                _scrapeReport.TotalRequestsMade++;

            return json;
        }

        internal async Task<NavItem> FetchItemsInternalAsync(string uri)
        {
            var basObj = Cache.FetchItem<NavItem>(uri);

            if (basObj == null)
            {

                var json = await FetchJsonAsync(uri).ConfigureAwait(false);

                if (string.IsNullOrEmpty(json))
                    return null;

                var replaceRootRegex = new Regex(RootRegex);

                var finalJson = replaceRootRegex.Replace(json, "\"root\"", 1);

                basObj = JsonConvert.DeserializeObject<NavItem>(finalJson);

                if (basObj != null)
                {
                    if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri));

                    Cache.PutItem(basObj, uri);
                }
            }
            else
            {
                if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri, true));
            }

            return basObj;
        }

        public async Task<NavItem> FetchNavItemAsync(string uri)
        {
            _scrapeCancelled = false;
            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));
            var items = await FetchItemsInternalAsync(uri).ConfigureAwait(false);
            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            return items;
        }

        private async Task RecParseAsync(string loc, List<string> list)
        {
            if (_scrapeCancelled)
                return;

           // await Task.Yield();
            var items = await FetchItemsInternalAsync(loc).ConfigureAwait(false);

            if (items != null)
            {
                foreach (var ou in items.Items.OrganizationalUnit ?? new Element[] {})
                {
                    await RecParseAsync(string.Format("{0}.json", ou.Value), list).ConfigureAwait(false);
                }

                foreach (var ou in items.Items.KnowledgeGrouping ?? new Element[] { })
                {
                    await RecParseAsync(string.Format("{0}.json", ou.Value), list).ConfigureAwait(false);
                }

                if (items.Items.UsesList.HasContent())
                {
                    list.AddRange(items.Items.UsesList.Select(n => n.Value));
                }
            }
        }

        public async Task<IEnumerable<string>> ScrapeReadingListsAsync(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Log.Error("Scraper.ParseTest: Could not initiate scrape. The root node address was empty.");
                return null;
            }
            _scrapeCancelled = false;
            var lists = new List<string>();
            var stopwatch = new Stopwatch();
            _scrapeReport = new ScrapeReport();

            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));

            _scrapeReport.ScrapeStarted = DateTime.Now;

            stopwatch.Start();
            await RecParseAsync(root, lists).ConfigureAwait(false);
            stopwatch.Stop();

            _scrapeReport.ScrapeEnded = DateTime.Now;
            _scrapeReport.TimeTaken = stopwatch.Elapsed;

            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            return lists;
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

            return json;
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

                var replaceRootRegex = new Regex(RootRegex);

                var finalJson = replaceRootRegex.Replace(json, "\"root\"", 1);

                basObj = JsonConvert.DeserializeObject<NavItem>(finalJson);

                if (basObj != null)
                {
                    Cache.PutItem(basObj, uri);
                    if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri));
                }

            }
            else
            {
                if (_scrapeReport != null)
                    _scrapeReport.TotalCacheRequestsMade++;

                if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri, true));
            }

            return basObj;
        }

        public NavItem FetchNavItem(string uri)
        {
            _scrapeCancelled = false;
            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));
            var items = FetchItemsInternal(uri);
            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

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

        public IEnumerable<string> ScrapeReadingLists(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Log.Fatal("Scraper.ParseTest: Could not initiate scrape. The root node address was empty.");
                return null;
            }

            _scrapeCancelled = false;
            var lists = new List<string>();
            var stopwatch = new Stopwatch();
            _scrapeReport = new ScrapeReport();

            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));

            _scrapeReport.ScrapeStarted = DateTime.Now;

            stopwatch.Start();
            RecParse(root, ref lists);
            stopwatch.Stop();

            _scrapeReport.ScrapeEnded = DateTime.Now;
            _scrapeReport.TimeTaken = stopwatch.Elapsed;

            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            return lists;
        }

        //todo: how does cancel scrape fit into this? Might pass a prescraped collection in, so can't assume we outright cancel it
        public IEnumerable<ReadingList> PopulateReadingLists(IEnumerable<string> readingLists)
        {//scrape lists from passed in uri collection of lists
            
            if (!readingLists.HasContent())
            {
                Log.Error("Attempted to populate reading lists, but passed in list object was null.");
                return null;
            }

            _scrapeCancelled = false;

            //TODO: need to fire resourceScraped event
            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.Books));
            _scrapeReport = new ScrapeReport();

            var readingListCollection = new Collection<ReadingList>();
            var stopWatch = new Stopwatch();

            _scrapeReport.ScrapeStarted = DateTime.Now;

            stopWatch.Start();
            foreach (var item in readingLists)
            {
                var uri = string.Format("{0}.json", item);

                var readingListNavObj = FetchItemsInternal(uri);

                if (readingListNavObj == null)
                {
                    Log.Error("Reading list from uri:{0} could not be scraped.", uri);
                    continue;
                }

                readingListCollection.Add(new ReadingList { Uri = uri, ListInfo = readingListNavObj});
                
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
                                            var bookObj = ParseBookInfoFromJson(bookItem);

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
            stopWatch.Start();

            _scrapeReport.ScrapeEnded = DateTime.Now;
            _scrapeReport.TimeTaken = stopWatch.Elapsed;

            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.Books));

            return readingListCollection;
        }

        //TODO: Flesh this out!
        private static Book ParseBookInfoFromJson(string json)
        {
            var book = new Book();

            var jObj = JObject.Parse(json);


            var rawOrg = jObj.Properties().FirstOrDefault(p => p.Name.Contains("/organisations/"));
            if (rawOrg.HasValues)
            {
                var publisher = JObject.FromObject(rawOrg); //rawOrg["http://xmlns.com/foaf/0.1/name"]["value"];
                var ttt = publisher.Properties().FirstOrDefault(p => p.Contains("/name"));
            }


            return new Book();
        }

        #endregion

        public bool CancelScrape()
        {
            _scrapeCancelled = true;

            if(ScrapeCancelled != null) ScrapeCancelled(this, new ScrapeCancelledEventArgs());

            return true;
        }

        public ScrapeReport FetchScrapeReport()
        {
            return _scrapeReport;
        }
    }
}
