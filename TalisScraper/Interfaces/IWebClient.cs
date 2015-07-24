using System;
using System.Net;
using System.Threading.Tasks;

namespace TalisScraper.Interfaces
{
    /// <summary>
    /// WebClient abstracted out to an interface to allow testing of events raised during WebClient failure.
    /// </summary>
    public interface IWebClient : IDisposable
    {
        WebHeaderCollection ResponseHeaders { get; }

        Task<string> DownloadStringTaskAsync(Uri uri);
        string DownloadString(Uri uri);

        WebClient WebClient { get; }
    }
}
