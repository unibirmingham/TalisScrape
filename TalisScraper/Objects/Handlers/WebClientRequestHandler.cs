using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using TalisScraper.Interfaces;

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

        public async Task<string> FetchJsonAsync(string uri)
        {
            using (var wc = new WebClient())
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
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return json;
            }
        }

        public string FetchJson(string uri)
        {
            using (var wc = new WebClient())
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
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return json;
            }
        }
    }
}
