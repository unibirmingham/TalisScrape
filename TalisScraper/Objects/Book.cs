using System.Collections.Generic;

namespace TalisScraper.Objects
{
    public class Book
    {
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string URL { get; set; }
        public bool IsOnlineResource { get; set; }
        public IEnumerable<string> Authors { get; set; } 
    }
}
