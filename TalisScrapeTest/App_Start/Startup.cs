using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TalisScrapeTest.Startup))]
namespace TalisScrapeTest
{
    
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}