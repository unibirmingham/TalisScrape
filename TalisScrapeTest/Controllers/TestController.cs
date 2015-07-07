using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Web.Mvc;
using NLog;
using TalisScraper;
using TalisScraper.Events;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;
using TalisScraper.Objects;
using TalisScraper.Objects.JsonMaps;

namespace TalisScrapeTest.Controllers
{
    public class TestController : Controller
    {
        private readonly IScraper _scraper;
        private readonly ILogger _log;
        public TestController()
        {
            _scraper = MvcApplication.Container.Resolve<IScraper>();
            _log = LogManager.GetCurrentClassLogger();
        }

        private void ScraperOnScrapeEnded(object sender, ScrapeEndedEventArgs args)
        {
            _log.Info("Scrape Ended: {0} (type: {1})<br/>", args.Ended, args.Type);
        }

        private void ScraperOnScrapeStarted(object sender, ScrapeStartedEventArgs args)
        {
            _log.Info("Scrape Started: {0} (type: {1})<br/>", args.Started, args.Type);
        }

        private void ScraperOnResourceScraped(object sender, ResourceScrapedEventArgs args)
        {
            _log.Info("Scraped: {0} (from cache: {1})", args.URI, args.FromCache);
        }

        public async Task<ActionResult> Index(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";//"http://aspire.aber.ac.uk/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;
            var baseItem = await _scraper.FetchNavItemAsync(name).ConfigureAwait(false);
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;

            return View(baseItem);
        }
        
        public async Task<ActionResult> DoScrape(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;

            var parseTest = await _scraper.ScrapeReadingListsAsync(name).ConfigureAwait(false);//Async();//pass root in here?

            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;
            ViewBag.ParseTest = parseTest;

            return View(new NavItem());
        }

        public ActionResult Index1(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;
            var parseTest = _scraper.FetchNavItem(name);
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;

            return View(parseTest);
        }


       /* public ActionResult Dynamic(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
            var baseItem = _scraper.FetchDyn(name);

            return System.Web.UI.WebControls.View(baseItem);
        }*/
    }
}