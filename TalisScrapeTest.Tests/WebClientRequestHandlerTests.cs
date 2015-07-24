using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TalisScraper.Exceptions;
using TalisScraper.Objects.Handlers;

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
    public class WebClientRequestHandlerTests
    {
        private WebClientRequestHandler _requestHandler;
        private Mock<WebClientHandler> _moqWebClient;

        private const string GenericJson = "{\"http://demo.talisaspire.com/\": {\"http://purl.org/vocab/aiiso/schema#name\": [{\"value\": \"Broadminster\",\"type\": \"literal\"}]}}";

        [SetUp]
        public void SetUp()
        {
            _requestHandler = new WebClientRequestHandler();
            _moqWebClient = new Mock<WebClientHandler>();
            _requestHandler._webClient = _moqWebClient.Object;
        }

        #region Sync Tests

        [Test]
        public void FetchJson_ValidUriValidEndPoint_ReturnsExpectedJson()
        {
            _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Returns(GenericJson);

            Assert.AreEqual(_requestHandler.FetchJson("http://testuri.com"), GenericJson);
        }

        [Test]
        public void FetchJson_ValidUriInValidEndPoint_ReturnsEmptyString()
        {
            _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Throws<WebException>();

            StringAssert.AreEqualIgnoringCase(_requestHandler.FetchJson("http://invalidEndpoint.com"), string.Empty);
        }

        [Test]
        public void FetchJson_ValidUriNoneJsonResponse_ThrowsInvalidDataException()
        {
            _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Returns("SomeContent");
            _moqWebClient.Setup(m => m.ResponseHeaders).Returns(new WebHeaderCollection { { "content-type", "fffff" } });

            Assert.Throws<InvalidResponseException>(() => _requestHandler.FetchJson("http://noneJsonEndpoint.com"));
        }

        [Test]
        public void FetchJson_InValidUri_ReturnsEmptyString()
        {
            _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Throws<UriFormatException>();

            StringAssert.AreEqualIgnoringCase(_requestHandler.FetchJson("InvalidUri"), string.Empty);
        }



        #endregion

        #region Async Tests

        [Test]
        public async Task FetchJsonAsync_ValidUriValidEndPoint_ReturnsExpectedJson()
        {
            _moqWebClient.Setup(m => m.DownloadStringTaskAsync(It.IsAny<Uri>())).ReturnsAsync(GenericJson);

            Assert.AreEqual(await _requestHandler.FetchJsonAsync("http://testuri.com"), GenericJson);
        }

        [Test]
        public async Task FetchJsonAsync_ValidUriInValidEndPoint_ReturnsEmptyString()
        {
            _moqWebClient.Setup(m => m.DownloadStringTaskAsync(It.IsAny<Uri>())).Throws<WebException>();

            var result = await _requestHandler.FetchJsonAsync("http://invalidEndpoint.com");

            StringAssert.AreEqualIgnoringCase(result, string.Empty);
        }

        [Test]
        public async Task FetchJsonAsync_ValidUriNoneJsonResponse_ThrowsInvalidDataException()
        {
            _moqWebClient.Setup(m => m.DownloadStringTaskAsync(It.IsAny<Uri>())).ReturnsAsync("SomeContent");
            _moqWebClient.Setup(m => m.ResponseHeaders).Returns(new WebHeaderCollection { { "content-type", "SomeOtherFormat" } });

            Assert.Throws<InvalidResponseException>(() => _requestHandler.FetchJson("http://noneJsonEndpoint.com"));
        }

        [Test]
        public async Task FetchJsonAsync_InValidUri_ReturnsEmptyString()
        {
            _moqWebClient.Setup(m => m.DownloadStringTaskAsync(It.IsAny<Uri>())).Throws<UriFormatException>();

            StringAssert.AreEqualIgnoringCase(_requestHandler.FetchJson("InvalidUri"), string.Empty);

          //  Assert.Throws<UriFormatException>(async () => await _requestHandler.FetchJsonAsync("InvalidUri"));
        }

        [Test]
        public async Task FetchNavItemAsync_ValidUriInValidEndPoint_RequestFailedEventRaised()
        {
            /*   _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).Throws<WebException>();
               _moqReqHandler.Setup(m => m.WebClient(new WebClient()));
               RequestFailedEventArgs eventArgs = null;

               _requestHandler.RequestFailed += (sender, args) =>
               {
                   eventArgs = args;
               };

               await _requestHandler.FetchJsonAsync("http://testuri.com");

               Assert.NotNull(eventArgs);*/
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            _requestHandler = null;
         //   _moqReqHandler = null;
        }
    }
}
