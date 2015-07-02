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
        // GET: Test
        public ActionResult Index()
        {//[DataMember(Name = "http://demo.talisaspire.com/")]
            var schools = _scraper.FetchItems("http://demo.talisaspire.com/index.json");

            return View(schools);
        }
    }
}