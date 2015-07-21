using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;

#if DEBUG
[assembly: InternalsVisibleTo("TalisScrapeTest.Tests")]
#endif
namespace TalisScraper.Objects.Handlers
{
    public class WebClientRequestHandler : IRequestHandler
    {
        private readonly ScrapeConfig _scrapeConfig;

        public WebClientRequestHandler(ScrapeConfig config = null)
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 300;

            Log = LogManager.GetCurrentClassLogger();

            _scrapeConfig = config;
        }

        public ILogger Log { get; set; }

        public event EventHandler<RequestFailedEventArgs> RequestFailed;

        /// <summary>
        /// A seam for injecting mock webClient for testing purposes
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        internal WebClient InstantiateWebClient(WebClient web = null)
        {
            return web ?? new WebClient();
        }

        public async Task<string> FetchJsonAsync(string uri)
        {
            using (var wc = InstantiateWebClient())
            {
                //info: not sure if this is needed so commented out. If scraping fails if run as service or exe, might be due to site filtering out non-browser trafic. We can spoof a header to overcome this.
               // wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
    
                var json = string.Empty;

                //if throttle has been configured, delay the function for specified time
                if (_scrapeConfig != null && _scrapeConfig.RequestThrottle > TimeSpan.Zero)
                    await Task.Delay(_scrapeConfig.RequestThrottle);

                try
                {
                    var convertedUri = new Uri(uri);
                    json = await wc.DownloadStringTaskAsync(convertedUri);

                    var type = wc.ResponseHeaders["content-type"];

                    //todo: shoudl we throw this here, or outside try?
                    if (string.IsNullOrEmpty(type) || !type.Contains("json"))
                        throw new InvalidDataException();

                }
                catch (WebException ex)
                {
                    var statusCode = string.Empty;

                        if(ex.Status == WebExceptionStatus.ProtocolError) 
                        {
                            var rawStatusCode = ((HttpWebResponse)ex.Response).StatusCode;

                            statusCode = rawStatusCode.ToString();
                        }

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, statusCode, ex.Message));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, string.Empty, ex.Message));
                }

                return json;
            }
        }

        public string FetchJson(string uri)
        {
            using (var wc = InstantiateWebClient())
            {
                var json = string.Empty;

                if (_scrapeConfig != null && _scrapeConfig.RequestThrottle > TimeSpan.Zero)
                    Thread.Sleep(_scrapeConfig.RequestThrottle);

                try
                {
                    var convertedUri = new Uri(uri);
                    json = wc.DownloadString(convertedUri);

                    var type = wc.ResponseHeaders["content-type"];

                    if (string.IsNullOrEmpty(type) || !type.Contains("json"))
                        throw new InvalidDataException();

                }
                catch (WebException ex)
                {
                    var statusCode = string.Empty;

                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var rawStatusCode = ((HttpWebResponse)ex.Response).StatusCode;

                        statusCode = rawStatusCode.ToString();
                    }

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, statusCode, ex.Message));
                }
                catch (Exception ex)
                {
                    Log.Error(ex);

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, string.Empty, ex.Message));
                }

                return json;
            }
        }
    }
}
