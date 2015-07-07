using System.Collections.Generic;
using TalisScraper.Objects.JsonMaps;

namespace TalisScraper.Objects
{
    public class ReadingList
    {
        public string Uri { get; set; }
        public NavItem ParentItem { get; set; }

        public IEnumerable<Book> Books { get; set; } 
    }
}
