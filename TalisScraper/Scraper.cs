using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TalisScraper.Objects;
using Cache;
using Extensions;
using NLog;
using TalisScraper.Enums;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;


namespace TalisScraper
{
    public class Scraper : IScraper
    {
        private const string RootRegex = "\"([^\"]+)\"";
        private readonly IRequestHandler _requestHandler;

        public Scraper(IRequestHandler requestHandler)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 300;

            Log = LogManager.GetCurrentClassLogger();//todo: inject this in?
            _requestHandler = requestHandler;
        }

        public ILogger Log { get; set; }
        public ICache Cache { get; set; }
        
        #region Async Functions
        public event EventHandler<ScrapeEndedEventArgs> ScrapeEnded;
        public event EventHandler<ScrapeStartedEventArgs> ScrapeStarted;
        public event EventHandler<ResourceScrapedEventArgs> ResourceScraped;

        /// <summary>
        /// Fetches json object from the specified uri using async
        /// </summary>
        /// <param name="uri">uri of json object</param>
        /// <returns>a string json object</returns>
        internal async Task<string> FetchJsonAsync(string uri)
        {
            return await _requestHandler.FetchJsonAsync(new Uri(uri));
        }

        internal async Task<NavItem> FetchItemsInternalAsync(string uri)
        {
            var basObj = Cache.FetchItem<NavItem>(uri);

            if (basObj == null)
            {

                var json = await FetchJsonAsync(uri);

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

        public async Task<NavItem> FetchItemsAsync(string uri)
        {
            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));
            var items = await FetchItemsInternalAsync(uri);
            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            return items;
        }

        private async Task RecParseAsync(string loc, List<ReadingList> list)
        {
           // await Task.Yield();
            var items = await FetchItemsAsync(loc);

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
                    list.AddRange(items.Items.UsesList.Select(n => new ReadingList {Uri = n.Value}));
                }
            }
        }

        public async Task<IEnumerable<ReadingList>> ScrapeReadingListsAsync(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Log.Error("Scraper.ParseTest: Could not initiate scrape. The root node address was empty.");
                return null;
            }

            var lists = new List<ReadingList>();

            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));

            await RecParseAsync(root, lists);

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
            return _requestHandler.FetchJson(new Uri(uri));
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

                if (string.IsNullOrEmpty(json))
                    return null;

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
                if (ResourceScraped != null) ResourceScraped(this, new ResourceScrapedEventArgs(uri, true));
            }

            return basObj;
        }

        public NavItem FetchItems(string uri)
        {
            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));
            var items = FetchItemsInternal(uri);
            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            return items;
        }

        private void RecParse(string loc, ref List<ReadingList> list)
        {
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
                    list.AddRange(items.Items.UsesList.Select(n => new ReadingList { Uri = n.Value }));
                }
            }

        }

        public IEnumerable<ReadingList> ScrapeReadingLists(string root)
        {
            if (string.IsNullOrEmpty(root))
            {
                Log.Fatal("Scraper.ParseTest: Could not initiate scrape. The root node address was empty.");
                return null;
            }

            var lists = new List<ReadingList>();

            if (ScrapeStarted != null) ScrapeStarted(this, new ScrapeStartedEventArgs(ScrapeType.ReadingList));
            RecParse(root, ref lists);
            if (ScrapeEnded != null) ScrapeEnded(this, new ScrapeEndedEventArgs(ScrapeType.ReadingList));

            return lists;
        }
        #endregion
    }
}
