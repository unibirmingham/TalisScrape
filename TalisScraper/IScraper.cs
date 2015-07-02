using TalisScraper.Objects;

namespace TalisScraper
{
    public interface IScraper
    {
        string FetchJson(string name = "");

        Items FetchItems(string items);
    }
}
