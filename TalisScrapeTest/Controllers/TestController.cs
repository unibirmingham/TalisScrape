using System.Web.Mvc;
using TalisScraper;

namespace TalisScrapeTest.Controllers
{
    public class TestController : Controller
    {
        private readonly IScraper _scraper;

        public TestController()
        {
            _scraper = MvcApplication.Container.Resolve<IScraper>();
        }

        public ActionResult Index(string id)
        {
            var name = id ?? "http://demo.talisaspire.com/index.json";
            var baseItem = _scraper.FetchItems(name);//"http://demo.talisaspire.com/index.json");

            return View(baseItem);
        }
    }
}