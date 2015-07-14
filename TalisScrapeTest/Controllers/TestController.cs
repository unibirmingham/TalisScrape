using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services;
using Extensions;
using Microsoft.AspNet.SignalR;
using NLog;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;
using TalisScraper.Objects;
using TalisScraper.Objects.JsonMaps;
using TalisScrapeTest.Hubs;

namespace TalisScrapeTest.Controllers
{
    public class TestController : Controller
    {
        private readonly IScraper _scraper;
        private readonly ILogger _log;
        private readonly IHubContext _scrapeHub;
        public TestController()
        {
            _scrapeHub = GlobalHost.ConnectionManager.GetHubContext<ScrapeHub>();
            _scraper = MvcApplication.Container.Resolve<IScraper>();
            _log = LogManager.GetCurrentClassLogger();
        }

        private void ScraperOnScrapeEnded(object sender, ScrapeEndedEventArgs args)
        {
            _scrapeHub.Clients.Group("doScrape").doMessage(string.Format("Scrape Ended: {0} (type: {1})", args.Ended, args.Type));
           // _log.Info("Scrape Ended: {0} (type: {1})<br/>", args.Ended, args.Type);
        }

        private void ScraperOnScrapeStarted(object sender, ScrapeStartedEventArgs args)
        {
            _scrapeHub.Clients.Group("doScrape").doMessage(string.Format("Scrape Started: {0} (type: {1})", args.Started, args.Type));
           // _log.Info("Scrape Started: {0} (type: {1})<br/>", args.Started, args.Type);
        }

        private void ScraperOnResourceScraped(object sender, ResourceScrapedEventArgs args)
        {
            _scrapeHub.Clients.Group("doScrape").doMessage(string.Format("Scraped: {0} (from cache: {1})", args.URI, args.FromCache));
           // _log.Info("Scraped: {0} (from cache: {1})", args.URI, args.FromCache);
        }


        private void ScraperOnScrapeCancelled(object sender, ScrapeCancelledEventArgs args)
        {
            _scrapeHub.Clients.Group("doScrape").doMessage(string.Format("Scraped Cancelled at {0}.", args.Ended));
        }

        public async Task<ActionResult> Index(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";//"http://aspire.aber.ac.uk/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;
            var baseItem = await _scraper.FetchNavItemAsync(name);
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;

            return View(baseItem);
        }
        
        public async Task<ActionResult> DoScrape(string id)
        {



            return View(new NavItem());
        }

        [WebMethod]
        public async Task InitiateScrape(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;
            _scraper.ScrapeCancelled += ScraperOnScrapeCancelled;

            var lists = await _scraper.ScrapeReadingListsAsync(name);//Async();//pass root in here?



            var listScrapeReport = _scraper.FetchScrapeReport();

            if (listScrapeReport != null)
                _scrapeHub.Clients.Group("scrapeReports").doReport(listScrapeReport.ToJson());
            

            ScrapeReport bookScrapeReport;

            if (lists.HasContent())
            {
                var tst =_scraper.PopulateReadingLists(lists);

                bookScrapeReport = _scraper.FetchScrapeReport();

                if (bookScrapeReport != null)
                    _scrapeHub.Clients.Group("scrapeReports").doReport(bookScrapeReport.ToJson());

                var export = _scraper.DoExport(tst);
            }


            
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;
            _scraper.ScrapeCancelled -= ScraperOnScrapeCancelled;

        }


        [WebMethod]
        public void CancelScrape()
        {
            _scraper.CancelScrape();
        }


        public ActionResult JObjTest()
        {
            var json = "";

            return View();
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