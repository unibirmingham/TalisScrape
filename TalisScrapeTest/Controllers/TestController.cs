using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Mvc;
using TalisScraper;
using TalisScraper.Objects;

namespace TalisScrapeTest.Controllers
{
    public class TestController : Controller
    {
        private readonly IScraper _scraper;

        public TestController()
        {
            _scraper = MvcApplication.Container.Resolve<IScraper>();
        }

        public async Task<ActionResult> Index(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
           // var baseItem = await _scraper.FetchItems(name);

            var stopwatch = new Stopwatch();
            
            stopwatch.Start();
            var parseTest = await _scraper.ParseTestAsync();//pass root in here?
            stopwatch.Stop();

            var ts = stopwatch.Elapsed;
            var elapsedTime = String.Format("{0}ms", ts.TotalMilliseconds);

            Response.Write(elapsedTime);

            ViewBag.ParseTest = parseTest;

            return View(new Base());
        }

        public ActionResult Index1(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
            // var baseItem = await _scraper.FetchItems(name);

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var parseTest = _scraper.ParseTest();//pass root in here?
            stopwatch.Stop();

            var ts = stopwatch.Elapsed;
            var elapsedTime = String.Format("{0}ms", ts.TotalMilliseconds);

            Response.Write(elapsedTime);

            ViewBag.ParseTest = parseTest;

            return View(new Base());
        }


       /* public ActionResult Dynamic(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
            var baseItem = _scraper.FetchDyn(name);

            return System.Web.UI.WebControls.View(baseItem);
        }*/
    }
}