using System;
using System.Net;

namespace TalisScraper.Objects
{
    /// <summary>
    /// Used to specify configuration options for the scrape
    /// </summary>
    public class ScrapeConfig
    {
        public ScrapeConfig()
        {
            RequestThrottle = TimeSpan.Zero;
          //  ServicePointManager.Expect100Continue = false;
           // ServicePointManager.DefaultConnectionLimit = 300;
        }

        /// <summary>
        /// Allows the webcrawl to be throttled - this might be needed for apis which only allow n requests per time period
        /// </summary>
        public TimeSpan RequestThrottle { get; set; }

    }
}
