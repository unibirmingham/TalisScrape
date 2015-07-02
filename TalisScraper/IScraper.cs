using TalisScraper.Objects;

namespace TalisScraper
{
    public interface IScraper
    {
        string FetchJson(string name = "");

        Base FetchItems(string items);

        T FetchItems<T>(string items);
    }
}
