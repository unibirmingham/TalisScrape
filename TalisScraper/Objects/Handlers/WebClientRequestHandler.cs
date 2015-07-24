using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TalisScraper.Events.Args;
using TalisScraper.Exceptions;
using TalisScraper.Interfaces;

#if DEBUG
[assembly: InternalsVisibleTo("TalisScrapeTest.Tests")]
#endif
namespace TalisScraper.Objects.Handlers
{
    public class WebClientRequestHandler : IRequestHandler
    {
        private readonly ScrapeConfig _scrapeConfig;

        internal IWebClient _webClient { get; set; }

        public WebClientRequestHandler()
        {
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 300;

            Log = LogManager.GetCurrentClassLogger();

            _scrapeConfig = null;
        }

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
        internal IWebClient WebClient()
        {
            return _webClient ?? new WebClientHandler();
        }

        public async Task<string> FetchJsonAsync(string uri)
        {
            using (var wc = WebClient())
            {
                //info: not sure if this is needed so commented out. If scraping fails if run as service or exe, might be due to site filtering out non-browser trafic. We can spoof a header to overcome this.
               // wc.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
    
                var json = string.Empty;
                var exceptionThrown = false;
                //if throttle has been configured, delay the function for specified time
                if (_scrapeConfig != null && _scrapeConfig.RequestThrottle > TimeSpan.Zero)
                    await Task.Delay(_scrapeConfig.RequestThrottle);

                try
                {
                    var convertedUri = new Uri(uri);
                    json = await wc.DownloadStringTaskAsync(convertedUri);




                }
                catch (WebException ex)
                {
                    exceptionThrown = true;
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
                    exceptionThrown = true;

                    Log.Error(ex);

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, string.Empty, ex.Message));
                }

                //Throwing this in the try causes it to be caught by the proceeding catch. 
                //If another exption has been thrown we don't want to throw this, as a grerater error has occurred.
                if (!exceptionThrown)
                {
                    var type = wc.ResponseHeaders["content-type"];

                    if (string.IsNullOrEmpty(type) || !type.Contains("json"))
                        throw new InvalidResponseException();
                }

                return json;
            }
        }

        public string FetchJson(string uri)
        {
            using (var wc = WebClient())
            {
                var json = string.Empty;
                var exceptionThrown = false;

                if (_scrapeConfig != null && _scrapeConfig.RequestThrottle > TimeSpan.Zero)
                    Thread.Sleep(_scrapeConfig.RequestThrottle);

                try
                {
                    var convertedUri = new Uri(uri);
                    json = wc.DownloadString(convertedUri);

                }
                catch (WebException ex)
                {
                    exceptionThrown = true;
                    var statusCode = string.Empty;

                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var rawStatusCode = ((HttpWebResponse) ex.Response).StatusCode;

                        statusCode = rawStatusCode.ToString();
                    }

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, statusCode, ex.Message));
                }
                catch (Exception ex)
                {
                    exceptionThrown = true;

                    if (Log != null) Log.Error(ex);

                    if (RequestFailed != null)
                        RequestFailed(this, new RequestFailedEventArgs(uri, string.Empty, ex.Message));

                }


                //Throwing this in the try causes it to be caught by the proceeding catch. 
                //If another exption has been thrown we don't want to throw this, as a grerater error has occurred.
                if (!exceptionThrown)
                {
                    var type = wc.ResponseHeaders["content-type"];

                    if (string.IsNullOrEmpty(type) || !type.Contains("json"))
                        throw new InvalidResponseException();
                }

                return json;
            }
        }
    }
}
