using System;
using System.Net;
using System.Text.RegularExpressions;
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

        public Base FetchItems(string name)
        {
            var json = FetchJson(name);

            if (string.IsNullOrEmpty(json))
                return null;
            //var ttt = "\"([^\\}]+)\"";
            var replaceRootRegex = new Regex("\"([^\"]+)\"");

            var finalJson = replaceRootRegex.Replace(json, "\"root\"", 1);

            return JsonConvert.DeserializeObject<Base>(finalJson);
        }

        public T FetchItems<T>(string name)
        {
            var json = FetchJson(name);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
