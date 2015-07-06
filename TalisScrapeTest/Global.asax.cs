using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Cache;
using Funq;
using TalisScraper;

namespace TalisScrapeTest
{
    public class MvcApplication : HttpApplication
    {
        public static Container Container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Container = new Container();

            Container.Register<ICache>(new DevNullCacheProvider());
            Container.Register<IScraper>(new Scraper
            {
                Cache = Container.Resolve<ICache>(),
            });
            

        }
    }
}
