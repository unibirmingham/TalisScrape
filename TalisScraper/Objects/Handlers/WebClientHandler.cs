using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TalisScraper.Interfaces;

#if DEBUG
[assembly: InternalsVisibleTo("TalisScrapeTest.Tests")]
#endif
namespace TalisScraper.Objects.Handlers
{
    public class WebClientHandler : IWebClient
    {
        internal WebClient _webClient;

        private bool _disposed;


        public WebClientHandler()
        {
            _webClient = new WebClient();
        }

        public virtual WebClient WebClient { get { return _webClient; } }


        public virtual WebHeaderCollection ResponseHeaders
        {
            get { return _webClient.ResponseHeaders; }
        }
        
        public virtual async Task<string> DownloadStringTaskAsync(Uri uri)
        {
            var result = string.Empty;
            try
            {
                result = await _webClient.DownloadStringTaskAsync(uri);
                var type = _webClient.ResponseHeaders["content-type"];

                if (string.IsNullOrEmpty(type) || !type.Contains("json"))
                    throw new InvalidDataException();
            }
            catch
            {
                throw;
            }
            return result;
        }

        public virtual string DownloadString(Uri uri)
        {
            var result = string.Empty;

            try
            {
                _webClient.DownloadString(uri);

                /*var type = _webClient.ResponseHeaders["content-type"];

                if (string.IsNullOrEmpty(type) || !type.Contains("json"))
                    throw new InvalidDataException();*/
            }
            catch
            {
                throw;
            }

            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Dispose managed resources.
                if (_webClient != null)
                    _webClient.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~WebClientHandler()
        {
            Dispose(false); //I am *not* calling you from Dispose, it's *not* safe
        }
    }
}
