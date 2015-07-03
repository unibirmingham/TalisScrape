using System.Collections.Generic;
using TalisScraper.Objects;

namespace TalisScraper
{
    public interface IScraper
    {
        string FetchJson(string name = "");

        Base FetchItems(string items);

        dynamic FetchDyn(string name);

        T FetchItems<T>(string items);

        IEnumerable<string> ParseTest();
    }
}
