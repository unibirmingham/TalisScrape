using System;
using System.Threading.Tasks;
using TalisScraper.Events.Args;

namespace TalisScraper.Interfaces
{
    public interface IRequestHandler
    {        /// <summary>
        /// Event fired when a scrape attempt fails
        /// </summary>
        event EventHandler<RequestFailedEventArgs> RequestFailed;

        Task<string> FetchJsonAsync(string uri);
        string FetchJson(string uri);
    }
}
