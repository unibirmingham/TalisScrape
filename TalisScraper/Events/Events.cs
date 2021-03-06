﻿//#########################################
//## Event Args are defined in this file ##
//#########################################
using System;
using System.Net;
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

    public class ScrapeCancelledEventArgs : EventArgs
    {
        public DateTime Ended { get; internal set; }

        public ScrapeCancelledEventArgs()
        {
            Ended = DateTime.Now;
        }
    }

    public class ScrapeFailedEventArgs : EventArgs
    {
        public string URI { get; internal set; }
        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public ScrapeFailedEventArgs(string uri, string statusCode = "", string errorMessage = "")
        {
            URI = uri;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }



    public class RequestFailedEventArgs : EventArgs
    {
        public string URI { get; internal set; }
        public string StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public RequestFailedEventArgs(string uri, string statusCode = "", string errorMessage = "")
        {
            URI = uri;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}
