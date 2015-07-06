using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TalisScraper.Events.Args;
using TalisScraper.Objects;

namespace TalisScraper.Interfaces
{
    /// <summary>
    /// Scraper to fetch and parse Talis Aspire json reading list information
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
        /// Fetches Base type navigation item parsed from json located at the specified uri using async
        /// </summary>
        /// <param name="uri">A uri to the json data to be parsed</param>
        /// <returns>A base type object</returns>
        Task<NavItem> FetchItemsAsync(string uri);
        /// <summary>
        /// Fetches all reading lists found recursively from the specified uri using async
        /// </summary>
        /// <param name="root">A uri to the json data to be scraped</param>
        /// <returns>A collection of reading lists</returns>
        Task<IEnumerable<ReadingList>> ScrapeReadingListsAsync(string root);

        /// <summary>
        /// Fetches Base type navigation item parsed from json located at the specified uri
        /// </summary>
        /// <param name="uri">A uri to the json data to be parsed</param>
        /// <returns>A base type object</returns>
        NavItem FetchItems(string uri);
        /// <summary>
        /// Fetches all reading lists found recursively from the specified uri
        /// </summary>
        /// <param name="root">A uri to the json data to be scraped</param>
        /// <returns>A collection of reading lists</returns>
        IEnumerable<ReadingList> ScrapeReadingLists(string root);
    }
}
