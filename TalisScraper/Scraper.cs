using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TalisScraper.Objects;
using Cache;
using Extensions;
using NLog;


namespace TalisScraper
{
    public class Scraper : IScraper
    {
        private const string RootDoc = "http://demo.talisaspire.com/index.json";
      //  private WebClient wc;

        public Scraper()
        {
            //   wc = new WebClient();
            Log = LogManager.GetCurrentClassLogger();
        }

        public ILogger Log { get; set; }
        public ICache Cache { get; set; }
        
        #region Async Functions
        public async Task<string> FetchJsonAsync(string name = "")
        {
           // await Task.Yield();
            using (var wc = new WebClient())
            {
                var json = string.Empty;

                try
                {
                    json = await wc.DownloadStringTaskAsync(new Uri(name));

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return json;
            }
        }

        public async Task<Base> FetchItemsAsync(string name)
        {
           // await Task.Yield();
            Base basObj = Cache.FetchItem<Base>(name);

            if (basObj == null)
            {

                var json = await FetchJsonAsync(name);

                if (string.IsNullOrEmpty(json))
                    return null;
                //var ttt = "\"([^\\}]+)\"";
                Regex replaceRootRegex = new Regex("\"([^\"]+)\"");//, RegexOptions.Compiled);

                var finalJson = replaceRootRegex.Replace(json, "\"root\"", 1);



                basObj = JsonConvert.DeserializeObject<Base>(finalJson);


                if (basObj != null)
                    Cache.PutItem(basObj, name);

            }

            return basObj;
        }

        private async Task RecParseAsync(string loc, List<string> list)
        {
           // await Task.Yield();
            var items = await FetchItemsAsync(loc);

            if (items != null)
            {
                foreach (var ou in items.Items.OrganizationalUnit ?? new Element[] {})
                {
                    await RecParseAsync(string.Format("{0}.json", ou.Value), list);
                }

                foreach (var ou in items.Items.KnowledgeGrouping ?? new Element[] { })
                {
                    await RecParseAsync(string.Format("{0}.json", ou.Value), list);
                }

                if (items.Items.UsesList.HasContent())
                {
                    list.AddRange(items.Items.UsesList.Select(n => n.Value));
                }
            }
        }

        public async Task<IEnumerable<string>> ParseTestAsync()
        {
           // await Task.Yield();
            var lists = new List<string>();

            await RecParseAsync(RootDoc, lists);

            return lists;
        }

        #endregion

        #region Sync Functions
        public string FetchJson(string name = "")
        {
            using (var wc = new WebClient())
            {
                var json = string.Empty;

                try
                {
                    json = wc.DownloadString(new Uri(name));

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }

                return json;
            }
        }

        public Base FetchItems(string name)
        {

            Base basObj = null;//Cache.FetchItem<Base>(name);

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


        private void recParse(string loc, ref List<string> list)
        {
            var items = FetchItems(loc);

            if (items != null)
            {
                foreach (var ou in items.Items.OrganizationalUnit ?? new Element[] { })
                {
                    recParse(string.Format("{0}.json", ou.Value), ref list);
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
        #endregion
    }
}
