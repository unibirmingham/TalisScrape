﻿using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
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

            Container.Register<IScraper>(new Scraper());
        }
    }
}
