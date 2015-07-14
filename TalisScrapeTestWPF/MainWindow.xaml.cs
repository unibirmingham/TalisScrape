using System.Windows;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;

namespace TalisScrapeTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IScraper _scraper;
        
        public MainWindow()
        {
            _scraper = App.Container.Resolve<IScraper>();
            

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;

            InitializeComponent();
        }

        private void ScraperOnResourceScraped(object sender, ResourceScrapedEventArgs args)
        {
            Test.Text  += string.Format("Scraped: {0} (from cache: {1})\n", args.URI, args.FromCache);
        }

        private void ScraperOnScrapeEnded(object sender, ScrapeEndedEventArgs args)
        {
            Test.Text += string.Format("Scrape Ended: {0} (type: {1})\n", args.Ended, args.Type);
        }

        private void ScraperOnScrapeStarted(object sender, ScrapeStartedEventArgs args)
        {
            Test.Text += string.Format("Scrape Started: {0} (type: {1})\n", args.Started, args.Type);
        }

        private void BtnDoScrape_OnClick(object sender, RoutedEventArgs e)
        {
            _scraper.ScrapeReadingListsAsync("http://demo.talisaspire.com/index.json");
        }

        private void BtnCancelScrape_Click(object sender, RoutedEventArgs e)
        {
            _scraper.CancelScrape();
        }

        ~MainWindow()
        {
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;
        }
    }
}
