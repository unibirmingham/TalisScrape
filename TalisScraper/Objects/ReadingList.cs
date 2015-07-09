using System.Collections.Generic;
using System.Collections.ObjectModel;
using TalisScraper.Objects.JsonMaps;

namespace TalisScraper.Objects
{
    public class ReadingList
    {
        public ReadingList()
        {
            Books = new Collection<Book>();
        }
        public string Uri { get; set; }
        public NavItem ListInfo { get; set; }

        public ICollection<Book> Books { get; set; } 
    }
}
