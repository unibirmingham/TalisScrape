using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Mvc;
using TalisScraper;
using TalisScraper.Events;
using TalisScraper.Events.Args;
using TalisScraper.Objects;

namespace TalisScrapeTest.Controllers
{
    public class TestController : Controller
    {
        private readonly IScraper _scraper;

        public TestController()
        {
            _scraper = MvcApplication.Container.Resolve<IScraper>();

          //  _scraper.ScrapeStarted += ScraperOnScrapeStarted;
          //  _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            
        }

        ~TestController()
        {
          //  _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
           // _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            
        }

        private static void ScraperOnScrapeEnded(object sender, ScrapeEndedEventArgs args)
        {
            System.Web.HttpContext.Current.Response.Write(string.Format("Scrape Ended: {0} (type: {1})<br/>", args.Ended, args.Type));
        }

        private static void ScraperOnScrapeStarted(object sender, ScrapeStartedEventArgs args)
        {
            System.Web.HttpContext.Current.Response.Write(string.Format("Scrape Started: {0} (type: {1})<br/>", args.Started, args.Type));
        }

        private static void ScraperOnResourceScraped(object sender, ResourceScrapedEventArgs args)
        {
            System.Web.HttpContext.Current.Response.Write(string.Format("Scraped: {0} (from cache: {1})", args.URI, args.FromCache));
        }

        public async Task<ActionResult> Index(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";//"http://aspire.aber.ac.uk/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;
            var baseItem = await _scraper.FetchItemsAsync(name);
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;

            return View(baseItem);
        }
        
        public ActionResult DoScrape(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;



            var parseTest = _scraper.ScrapeReadingLists(name);//Async();//pass root in here?

            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;

            ViewBag.ParseTest = parseTest;



            return View(new NavItem());
        }

        public ActionResult Index1(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
            // var baseItem = await _scraper.FetchItems(name);

            var stopwatch = new Stopwatch();

            stopwatch.Start();
           // var parseTest = _scraper.ParseTest(name);//pass root in here?
            stopwatch.Stop();

            var ts = stopwatch.Elapsed;
            var elapsedTime = String.Format("{0}ms", ts.TotalMilliseconds);

            Response.Write(elapsedTime);

           // ViewBag.ParseTest = parseTest;

            return View(new NavItem());
        }


       /* public ActionResult Dynamic(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
            var baseItem = _scraper.FetchDyn(name);

            return System.Web.UI.WebControls.View(baseItem);
        }*/
    }
}