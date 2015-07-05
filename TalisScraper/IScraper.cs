using System.Collections.Generic;
using System.Threading.Tasks;
using TalisScraper.Events;
using TalisScraper.Objects;

namespace TalisScraper
{
    public interface IScraper
    {

        event ResourceScrapedHandler ResourceScraped;
        Task<string> FetchJsonAsync(string name = "");
        Task<Base> FetchItemsAsync(string items);
        Task<IEnumerable<string>> ParseTestAsync();

        string FetchJson(string name = "");
        Base FetchItems(string items);
        IEnumerable<string> ParseTest();
    }
}
