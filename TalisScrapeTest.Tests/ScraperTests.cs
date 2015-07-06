using System;
using System.Linq;
using System.Threading.Tasks;
using Cache;
using Moq;
using NUnit.Framework;
using TalisScraper;
using TalisScraper.Interfaces;
using TalisScraper.Objects;


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
    public class ScraperTests
    {
        private IScraper _scraper;
        private IRequestHandler _requestHandler;
        private readonly Mock<IRequestHandler> _moqReqHandler = new Mock<IRequestHandler>();

        private const string GenericJson = "{\"http://demo.talisaspire.com/\": {\"http://purl.org/vocab/aiiso/schema#name\": [{\"value\": \"Broadminster\",\"type\": \"literal\"}]}}";

        private readonly NavItem _generciNavItem = new NavItem { Items = new Items { Name = new[] { new Element { Type = "literal", Value = "Broadminster" } } } };

        [SetUp]
        public void SetUp()
        {
            _requestHandler = _moqReqHandler.Object;
            _scraper = new Scraper(_requestHandler) { Cache = new DevNullCacheProvider() };
        }

        [Test]
        public void FetchItems_ValidUri_ReturnsExpectedStringJsonObject()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<Uri>())).Returns(GenericJson);
            Assert.AreEqual(_scraper.FetchItems("http://testuri.com").Items.Name.First().Value, _generciNavItem.Items.Name.First().Value);
        }

        [Test]
        public void FetchItems_InValidUri_ThrowsUriFormatException()
        {
            _moqReqHandler.Setup(m => m.FetchJson(It.IsAny<Uri>())).Returns(GenericJson);
            Assert.Throws<UriFormatException>(() => _scraper.FetchItems("InvalidUri"));
        }

        [Test]
        public void FetchItemsAsync_ValidUri_ReturnsExpectedStringJsonObject()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<Uri>())).Returns(Task.FromResult(GenericJson));
            Assert.AreEqual(_scraper.FetchItems("http://testuri.com").Items.Name.First().Value, _generciNavItem.Items.Name.First().Value);
        }

        [Test]
        public void FetchItemsAsync_InValidUri_ThrowsUriFormatException()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<Uri>())).Returns(Task.FromResult(GenericJson));
            Assert.Throws<UriFormatException>(() => _scraper.FetchItemsAsync("InvalidUri"));
        }
        /*
        [Test]
        public void FetchSchools_JsonParsedIntoSchoolObject_SchoolObjectNotNull()
        {
            _scraper.Json = genericJson;

            Assert.NotNull(_scraper.FetchItems("afsdsf"));
        }

        public class Cat
        {
            public string Name { get; set; }
        }

        [Test]
        public void FetchSchools_JsonParsedIntoSchoolObject_SchoolObjectTest1NotNull()
        {
            _scraper.Json = genericJson;//"{\"22\":[{\"Value\":\"val1\",\"Type\":\"val2\"}]}";

            Assert.NotNull(_scraper.FetchItems("dsfds"));
        }*/


        [TearDown]
        public void TearDown()
        {
            _scraper = null;
        }
    }
}
