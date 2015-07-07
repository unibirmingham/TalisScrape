using System.Threading.Tasks;

namespace TalisScraper.Interfaces
{
    public interface IRequestHandler
    {
        Task<string> FetchJsonAsync(string uri);
        string FetchJson(string uri);
    }
}
