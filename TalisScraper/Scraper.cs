using System;
using System.Net;
using Newtonsoft.Json;
using TalisScraper.Objects;

namespace TalisScraper
{
    public class Scraper : IScraper
    {
        public string FetchJson(string name = "")
        {
            using (var wc = new WebClient())
            {
                var json = string.Empty;

                try
                {
                    json = wc.DownloadString(new Uri(name));

                }
                catch (Exception)
                {
                    //todo: log error
                }

                return json;
            }
        }

        public Items FetchItems(string name)
        {
            var json = FetchJson(name);

            if (string.IsNullOrEmpty(json))
                return null;

            return JsonConvert.DeserializeObject<Items>(json);
        }
    }
}
