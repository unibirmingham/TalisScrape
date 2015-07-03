using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TalisScraper.Objects;
using System.Web.Script.Serialization;
using Cache;
using Extensions;


namespace TalisScraper
{
    public class Scraper : IScraper
    {
        private const string RootDoc = "http://demo.talisaspire.com/index.json";
        
        public ICache Cache { get; set; }

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

            var basObj = Cache.FetchItem<Base>(name);

            if (basObj == null)
            {

                var json = FetchJson(name);

                if (string.IsNullOrEmpty(json))
                    return null;
                //var ttt = "\"([^\\}]+)\"";
                var replaceRootRegex = new Regex("\"([^\"]+)\"");

                var finalJson = replaceRootRegex.Replace(json, "\"root\"", 1);



                basObj = JsonConvert.DeserializeObject<Base>(finalJson);


                if (basObj != null)
                    Cache.PutItem(basObj, name);

            }

            return basObj;
        }

        public dynamic FetchDyn(string name)
        {
            throw new NotImplementedException();
            /* var json = FetchJson(name);

            if (string.IsNullOrEmpty(json))
                return null;

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            dynamic obj = serializer.Deserialize(json, typeof(object));

            return obj;*/
        }

        public T FetchItems<T>(string name)
        {
            var json = FetchJson(name);

            return JsonConvert.DeserializeObject<T>(json);
        }

        private void recParse(string loc, ref List<string> list)
        {
            var items = FetchItems(loc);

            if (items != null)
            {
                foreach (var ou in items.Items.OrganizationalUnit ?? new Element[] {})
                {
                    recParse(string.Format("{0}.json",ou.Value), ref list);
                }

                foreach (var ou in items.Items.KnowledgeGrouping ?? new Element[] { })
                {
                    recParse(string.Format("{0}.json", ou.Value), ref list);
                }

                if (items.Items.UsesList.HasContent())
                {
                    list.AddRange(items.Items.UsesList.Select(n => n.Value));
                }
            }

        }

        public IEnumerable<string> ParseTest()
        {
            var lists = new List<string>();

            recParse(RootDoc, ref lists);

            return lists;
        }
    }
}
