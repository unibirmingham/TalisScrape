using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Cache;
using Funq;
using TalisScraper;
using TalisScraper.Interfaces;
using TalisScraper.Objects.Handlers;

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
            Container.Register<IRequestHandler>(new WebClientRequestHandler());
            Container.Register<IExportHandler>(new OaiPmhXmlHandler());
            Container.Register<IScraper>(new JsonScraper(Container.Resolve<IRequestHandler>())
            {
                Cache = Container.Resolve<ICache>(),
                ExportHandler = Container.Resolve<IExportHandler>()
            });
            

        }
    }
}
