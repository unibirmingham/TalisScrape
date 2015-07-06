//#######################################################
//## Delegates and Event Args are defined in this file ##
//#######################################################
using System;
using TalisScraper.Enums;

namespace TalisScraper.Events.Args
{//Args for scrape events
    public class ResourceScrapedEventArgs : EventArgs
    {
        public string URI { get; internal set; }
        public bool FromCache { get; internal set; }

        public ResourceScrapedEventArgs(string uri, bool fromCache = false)
        {
            URI = uri;            
            FromCache = fromCache;
        }
    }

    public class ScrapeStartedEventArgs : EventArgs
    {
        public ScrapeType Type { get; internal set; }
        public DateTime Started { get; internal set; }

        public ScrapeStartedEventArgs(ScrapeType type)
        {
            Type = type;
            Started = DateTime.Now;
        }
    }

    public class ScrapeEndedEventArgs : EventArgs
    {
        public ScrapeType Type { get; internal set; }
        public DateTime Ended { get; internal set; }

        public ScrapeEndedEventArgs(ScrapeType type)
        {
            Type = type;
            Ended = DateTime.Now;
        }
    }
}
