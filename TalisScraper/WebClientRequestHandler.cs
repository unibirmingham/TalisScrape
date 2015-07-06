using System;
using System.Net;
using System.Threading.Tasks;
using NLog;
using TalisScraper.Interfaces;

namespace TalisScraper
{
    public class WebClientRequestHandler : IRequestHandler
    {
        public WebClientRequestHandler()
        {
            Log = LogManager.GetCurrentClassLogger();
        }

        public ILogger Log { get; set; }

        public async Task<string> FetchJsonAsync(Uri uri)
        {
            using (var wc = new WebClient())
            {
                var json = string.Empty;

                try
                {
                    json = await wc.DownloadStringTaskAsync(uri);


                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return json;
            }
        }

        public string FetchJson(Uri uri)
        {
            using (var wc = new WebClient())
            {
                var json = string.Empty;

                try
                {
                    json = wc.DownloadString(uri);

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
