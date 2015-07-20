using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cache;
using Moq;
using NUnit.Framework;
using TalisScraper;
using TalisScraper.Events.Args;
using TalisScraper.Interfaces;
using TalisScraper.Objects.JsonMaps;


namespace TalisScrapeTest.Tests
{
    /*
    * ################################################
    * ############# NAMING CONVENTION ################
    * ################################################
    * # MethodName_StateUnderTest_ ExpectedBehaviour #
    * ################################################
    */

    [TestFixture]
    public class JsonScraperTests
    {
        private JsonScraper _scraper;
        private IRequestHandler _requestHandler;
        private Mock<IRequestHandler> _moqReqHandler;// new Mock<IRequestHandler>();

        private const string GenericJson = "{\"http://demo.talisaspire.com/\": {\"http://purl.org/vocab/aiiso/schema#name\": [{\"value\": \"Broadminster\",\"type\": \"literal\"}]}}";
        private const string ValidUri = "http://www.someUri.com";
        private const string InvalidUri = "Nsddddddi23-p";

        private readonly NavItem _generciNavItem = new NavItem { Items = new Items { Name = new[] { new Element { Type = "literal", Value = "Broadminster" } } } };

        [SetUp]
        public void SetUp()
        {
            _moqReqHandler = new Mock<IRequestHandler>();
            _requestHandler = _moqReqHandler.Object;
            _scraper = new JsonScraper(_requestHandler) { Cache = new DevNullCacheProvider() };
        }

        #region Sync Tests
        [Test]
        public void FetchNavItem_ValidUriValidEndPoint_ReturnsExpectedNavItem()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            Assert.AreEqual(_scraper.FetchNavItem(ValidUri).Items.Name.First().Value, _generciNavItem.Items.Name.First().Value);
        }

        [Test]
        public void FetchNavItem_ValidUriInValidEndPoint_ReturnsNull()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(string.Empty);

            Assert.IsNull(_scraper.FetchNavItem(InvalidUri));
        }

        //todo: should we actually mock response, and check for json content type?
        [Test]
        public void FetchNavItem_ValidUriNoneJsonResponse_ThrowsInvalidDataException()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Throws<InvalidDataException>();

            Assert.Throws<InvalidDataException>(() => _scraper.FetchNavItem(InvalidUri));
        }

        [Test]
        public void FetchNavItem_InValidUri_ThrowsUriFormatException()
        {
            _moqReqHandler.Setup(m => m.FetchJson("InvalidUri")).Returns(string.Empty);
            Assert.IsNull(_scraper.FetchNavItem("InvalidUri"));
        }

        [Test]
        public void FetchNavItem_ScrapeStarted_ScrapeStartedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ScrapeStartedEventArgs eventArgs = null;

            _scraper.ScrapeStarted += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.FetchNavItem("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public void FetchNavItem_ScrapeEnded_ScrapeEndedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ScrapeEndedEventArgs eventArgs = null;

            _scraper.ScrapeEnded += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.FetchNavItem("http://testuricom");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public void FetchNavItem_ResourceScraped_ResourceScrapedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.FetchNavItem("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public void FetchNavItem_ResourceScrapedReportsCorrectUri_CorrectUriInEventArgs()
        {
            const string uri = "http://testuri.com";

            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.FetchNavItem(uri);

            Assert.AreEqual(eventArgs.URI, uri);
        }

        [Test]
        public void ScrapeReadingLists_ScrapeStarted_ScrapeStartedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ScrapeStartedEventArgs eventArgs = null;

            _scraper.ScrapeStarted += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.ScrapeReadingLists("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public void ScrapeReadingLists_ScrapeEnded_ScrapeEndedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ScrapeEndedEventArgs eventArgs = null;

            _scraper.ScrapeEnded += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.ScrapeReadingLists("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public void ScrapeReadingLists_ResourceScrapedReportsCorrectUri_CorrectUriInEventArgs()
        {
            const string uri = "http://testuri.com";

            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;
            };

            _scraper.ScrapeReadingLists(uri);

            StringAssert.AreEqualIgnoringCase(eventArgs.URI, uri);
        }

        [Test]
        public void ScrapeReadingLists_ScrapeThreeResources_ScrapeEventFiredThreeTimes()
        {
          //  const string uri = "http://testuri.com";

            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<string>())).Returns(GenericJson);
            
            ResourceScrapedEventArgs eventArgs = null;
            var counter = 0;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;

                counter++;
            };

            //_scraper.ScrapeReadingList calls FetchItemsInternal internally per json object parsed, so to test, we can call the internal func here
            for (int i = 0; i < 3; i++)
            {
                _scraper.FetchItemsInternal(ValidUri);
            }
            
            Assert.AreEqual(counter, 3);
        }
        #endregion


        #region async Tests
        [Test]
        public async Task FetchNavItemAsync_ValidUriValidEndPoint_ReturnsExpectedNavItem()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);
            var result = await _scraper.FetchNavItemAsync("http://testuri.com");
            Assert.AreEqual(result.Items.Name.First().Value, _generciNavItem.Items.Name.First().Value);
        }

        [Test]
        public async Task FetchNavItemAsync_ValidUriInValidEndPoint_ReturnsExpectedNavItem()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync("http://invalidEndpoint.com")).Throws<WebException>();

            Assert.Throws<WebException>(async () => await _scraper.FetchNavItemAsync("http://invalidEndpoint.com"));
        }

        //todo: should we actually mock response, and check for json content type?
        [Test]
        public async Task FetchNavItemAsync_ValidUriNoneJsonResponse_ThrowsInvalidDataException()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync("http://noneJsonEndpoint.com")).Throws<InvalidDataException>();

            Assert.Throws<InvalidDataException>(async () => await _scraper.FetchNavItemAsync("http://noneJsonEndpoint.com"));
        }

        [Test]
        public async Task FetchNavItemAsync_InValidUri_ThrowsUriFormatException()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync("InvalidUri")).ReturnsAsync(string.Empty);
            Assert.IsNull(await _scraper.FetchNavItemAsync("InvalidUri"));
        }

        [Test]
        public async Task FetchNavItemAsync_ScrapeStarted_ScrapeStartedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ScrapeStartedEventArgs eventArgs = null;

            _scraper.ScrapeStarted += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.FetchNavItemAsync("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public async Task FetchNavItemAsync_ScrapeEnded_ScrapeEndedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ScrapeEndedEventArgs eventArgs = null;

            _scraper.ScrapeEnded += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.FetchNavItemAsync("http://testuricom");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public async Task FetchNavItemAsync_ResourceScraped_ResourceScrapedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.FetchNavItemAsync("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public async Task FetchNavItemAsync_ResourceScrapedReportsCorrectUri_CorrectUriInEventArgs()
        {
            const string uri = "http://testuri.com";

            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.FetchNavItemAsync(uri);

            Assert.AreEqual(eventArgs.URI, uri);
        }

        [Test]
        public async Task ScrapeReadingListsAsync_ScrapeStarted_ScrapeStartedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ScrapeStartedEventArgs eventArgs = null;

            _scraper.ScrapeStarted += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.ScrapeReadingListsAsync("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public async Task ScrapeReadingListsAsync_ScrapeEnded_ScrapeEndedEventRaised()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ScrapeEndedEventArgs eventArgs = null;

            _scraper.ScrapeEnded += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.ScrapeReadingListsAsync("http://testuri.com");

            Assert.NotNull(eventArgs);
        }

        [Test]
        public async Task ScrapeReadingListsAsync_ResourceScrapedReportsCorrectUri_CorrectUriInEventArgs()
        {
            const string uri = "http://testuri.com";

            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;
            };

            await _scraper.ScrapeReadingListsAsync(uri);

            Assert.AreEqual(eventArgs.URI, uri);
        }

        [Test]
        public async Task ScrapeReadingListsAsync_ScrapeThreeResources_ScrapeEventFiredThreeTimes()
        {
            const string uri = "http://testuri.com";

            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);

            ResourceScrapedEventArgs eventArgs = null;
            var counter = 0;

            _scraper.ResourceScraped += (sender, args) =>
            {
                eventArgs = args;

                counter++;
            };
            
            //_scraper.ScrapeReadingList calls FetchItemsInternal internally per json object parsed, so to test, we can call the internal func here
            for (int i = 0; i < 3; i++)
            {
                await _scraper.FetchItemsInternalAsync(ValidUri);
            }

            Assert.AreEqual(counter, 3);
        }
        #endregion

        [TearDown]
        public void TearDown()
        {
            _moqReqHandler = null;
            _requestHandler = null;
            _scraper = null;
        }
    }
}
