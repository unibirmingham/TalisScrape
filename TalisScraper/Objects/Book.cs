using System;
using System.Collections.Generic;

namespace TalisScraper.Objects
{
    /// <summary>
    /// Encapsulates a book, journal or published article
    /// </summary>
    public class Book
    {
        public Book()
        {
            Isbn10 = new List<string>();
            Isbn13 = new List<string>();
            Authors = new List<string>();
            Subject = new List<string>();
            Source = new List<string>();
            Date = new List<string>();
            //set to min, so we can tell if a pase error occurred or the parse source didn't contain date information
        }
        public string Title { get; internal set; }
        public string Publisher { get; internal set; }
        public List<string> Subject { get; internal set; }
        public List<string> Date { get; internal set; }
        public List<string> Isbn10 { get; internal set; }
        public List<string> Isbn13 { get; internal set; }
        public string Url { get; internal set; }
        public string PlaceOfPublication { get; internal set; }
        public string Type { get; internal set; }
        public bool IsOnlineResource { get; internal set; }
        public List<string> Source { get; internal set; }
        public List<string> Authors { get; internal set; } 
    }
}
