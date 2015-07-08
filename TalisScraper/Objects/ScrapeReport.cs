using System;
using System.Collections.ObjectModel;

namespace TalisScraper.Objects
{
    /// <summary>
    /// A report containing statistics detailing the most recent scrape
    /// </summary>
    public class ScrapeReport
    {
        public ScrapeReport()
        {
            FailedScrapes = new Collection<string>();
            TotalRequestsMade = 0;
            TotalCacheRequestsMade = 0;
        }
        public TimeSpan TimeTaken { get; internal set; }
        public DateTime ScrapeStarted { get; internal set; }
        public DateTime ScrapeEnded { get; set; }

        public Collection<string> FailedScrapes { get; internal set; }

        public int TotalRequestsMade { get; set; }
        public int TotalCacheRequestsMade { get; set; }

    }
}
