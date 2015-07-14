using System.Windows;
using Cache;
using Funq;
using TalisScraper;
using TalisScraper.Interfaces;
using TalisScraper.Objects.Handlers;

namespace TalisScrapeTestWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Container Container;

        public App()
        {
            Container = new Container();

            Container.Register<ICache>(new DevNullCacheProvider());
            Container.Register<IRequestHandler>(new WebClientRequestHandler());
            Container.Register<IExportHandler>(new OaiPmhXmlHandler());
            Container.Register<IScraper>(new JsonScraper(Container.Resolve<IRequestHandler>()/*, new ScrapeConfig { EnableParallelProcessing = true }*/)
            {
                Cache = Container.Resolve<ICache>(),
                ExportHandler = Container.Resolve<IExportHandler>()
            });
        }

    }
}
