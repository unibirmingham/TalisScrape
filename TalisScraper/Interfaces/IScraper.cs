using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalisScraper.Events.Args;
using TalisScraper.Objects;
using TalisScraper.Objects.JsonMaps;

namespace TalisScraper.Interfaces
{
    /// <summary>
    /// Scraper to fetch and parse Talis Aspire reading list information
    /// </summary>
    public interface IScraper
    {
        /// <summary>
        /// Event fired when a single resource is scraped
        /// </summary>
        event EventHandler<ResourceScrapedEventArgs> ResourceScraped;
        /// <summary>
        /// Event fired when scrape is started
        /// </summary>
        event EventHandler<ScrapeStartedEventArgs> ScrapeStarted;
        /// <summary>
        /// Event fired when scrape ends
        /// </summary>
        event EventHandler<ScrapeEndedEventArgs> ScrapeEnded;
        /// <summary>
        /// Event fired when scrape is forcibly stopped
        /// </summary>
        event EventHandler<ScrapeCancelledEventArgs> ScrapeCancelled;

        /// <summary>
        /// Event fired when a scrape attempt fails
        /// </summary>
        event EventHandler<ScrapeFailedEventArgs> ScrapeFailed;

        /// <summary>
        /// Fetches Base type navigation item parsed from json located at the specified uri using async
        /// </summary>
        /// <param name="uri">A uri to the json data to be parsed</param>
        /// <returns>A base type object</returns>
        Task<NavItem> FetchNavItemAsync(string uri);

        /// <summary>
        /// Fetches all reading lists found recursively from the specified uri using async
        /// </summary>
        /// <param name="root">A uri to the json data to be scraped</param>
        /// <returns>A collection of reading lists</returns>
        Task<IEnumerable<string>> ScrapeReadingListsAsync(string root);

        /// <summary>
        /// Populates ReadList objects with Book Items found at the uri specified in the passed collection of uris using async
        /// </summary>
        /// <param name="readingLists">A collection of uris pointing to Book Items</param>
        /// <returns>A collection of populated ReadingList objects</returns>
        Task<IEnumerable<ReadingList>> PopulateReadingListsAsync(IEnumerable<string> readingLists);

        /// <summary>
        /// Fetches Base type navigation item parsed from json located at the specified uri
        /// </summary>
        /// <param name="uri">A uri to the json data to be parsed</param>
        /// <returns>A base type object</returns>
        NavItem FetchNavItem(string uri);
        /// <summary>
        /// Fetches all reading lists found recursively from the specified uri
        /// </summary>
        /// <param name="root">A uri to the json data to be scraped</param>
        /// <returns>A collection of reading lists</returns>
        IEnumerable<string> ScrapeReadingLists(string root);

        /// <summary>
        /// Populates ReadList objects with Book Items found at the uri specified in the passed collection of uris
        /// </summary>
        /// <param name="readingLists">A collection of uris pointing to Book Items</param>
        /// <returns>A collection of populated ReadingList objects</returns>
        IEnumerable<ReadingList> PopulateReadingLists(IEnumerable<string> readingLists);

        /// <summary>
        /// returns a string export of populated ReadingList objects
        /// </summary>
        /// <param name="readingLists"></param>
        /// <returns></returns>
        string DoExport(IEnumerable<ReadingList> readingLists);

        /// <summary>
        /// Force the scrape to cancel
        /// </summary>
        /// <returns></returns>
        bool CancelScrape();

        /// <summary>
        /// Fetchs a report detailing statistics of the last scrape done
        /// </summary>
        /// <returns>A scrape report object</returns>
        ScrapeReport FetchScrapeReport();
    }
}
