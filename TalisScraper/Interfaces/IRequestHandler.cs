using System;
using System.Threading.Tasks;

namespace TalisScraper.Interfaces
{
    public interface IRequestHandler
    {
        Task<string> FetchJsonAsync(Uri uri);
        string FetchJson(Uri uri);
    }
}
