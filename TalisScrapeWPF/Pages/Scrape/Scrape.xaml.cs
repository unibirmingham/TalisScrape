using System.Collections.Generic;
using System.Windows;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;

namespace TalisScrapeWPF.Pages.Scrape
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Scrape
    {
        private readonly IScraper _scraper;

        private IEnumerable<string> _readingLists; 

        
        public Scrape()
        {
            _scraper = App.Container.Resolve<IScraper>();
            

            _scraper.ScrapeStarted += ScraperOnScrapeStarted;
            _scraper.ScrapeEnded += ScraperOnScrapeEnded;
            _scraper.ResourceScraped += ScraperOnResourceScraped;

            InitializeComponent();
        }

        private void ScraperOnResourceScraped(object sender, ResourceScrapedEventArgs args)
        {
            Test.Text = Test.Text.Insert(0, string.Format("Scraped: {0} (from cache: {1})\n", args.URI, args.FromCache));
            
        }

        private void ScraperOnScrapeEnded(object sender, ScrapeEndedEventArgs args)
        {
            Test.Text = Test.Text.Insert(0, string.Format("Scrape Ended: {0} (type: {1})\n", args.Ended, args.Type));
            UpdateStatus(string.Format("Finished {0} Scrape", args.Type));
        }

        private void ScraperOnScrapeStarted(object sender, ScrapeStartedEventArgs args)
        {
           Test.Text = Test.Text.Insert(0, string.Format("Scrape Started: {0} (type: {1})\n", args.Started, args.Type));
           UpdateStatus(string.Format("Scraping {0} Objects", args.Type));
        }

        private async void BtnDoScrape_OnClick(object sender, RoutedEventArgs e)
        {
            _readingLists = await _scraper.ScrapeReadingListsAsync("http://demo.talisaspire.com/index.json");
        }

        private void BtnCancelScrape_Click(object sender, RoutedEventArgs e)
        {
            _scraper.CancelScrape();
        }

        private void UpdateStatus(string message)
        {
           // SbStatus.Text = message;
        }

        ~Scrape()
        {
            _scraper.ScrapeStarted -= ScraperOnScrapeStarted;
            _scraper.ScrapeEnded -= ScraperOnScrapeEnded;
            _scraper.ResourceScraped -= ScraperOnResourceScraped;
        }
    }
}
