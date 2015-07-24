using System;
using Moq;
using NUnit.Framework;
using TalisScraper.Interfaces;
using TalisScraper.Objects.Handlers;

namespace TalisScrapeTest.Tests
{    /*
    * ################################################
    * ############# NAMING CONVENTION ################
    * ################################################
    * # MethodName_StateUnderTest_ExpectedBehaviour #
    * ################################################
    */

    [TestFixture]
    public class WebClientHandlerTests
    {
        private Mock<WebClientHandler> _moqWebClientHandler;
        private WebClientHandler _webClientHandler;

        [SetUp]
        public void SetUp()
        {
            _moqWebClientHandler = new Mock<WebClientHandler>();

            _webClientHandler = _moqWebClientHandler.Object;
        }

        [Test]
        public void DownloadString_ValidUriValidEndPoint_ReturnsExpectedJson()
        {
         //   _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Returns(GenericJson);

        //    Assert.AreEqual(_requestHandler.FetchJson("http://testuri.com"), GenericJson);


        }

        [Test]
        public void DownloadString_ValidUriInValidEndPoint_ReturnsEmptyString()
        {
         //   _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Throws<WebException>();

         //   StringAssert.AreEqualIgnoringCase(_requestHandler.FetchJson("http://invalidEndpoint.com"), string.Empty);
        }

        [Test]
        public void DownloadString_ValidUriNoneJsonResponse_ReturnsEmptyString()
        {
          //  _moqWebClient.Setup(m => m.DownloadString(It.IsAny<Uri>())).Throws<InvalidDataException>();
         //   _moqWebClient.Setup(m => m.ResponseHeaders).Returns(new WebHeaderCollection { { "content-type", "NotJson" } });

         //   StringAssert.AreEqualIgnoringCase(_requestHandler.FetchJson("http://noneJsonEndpoint.com"), string.Empty);
        }

        [Test]
        public void DownloadString_InValidUri_ThorwsAnException()
        {
            _moqWebClientHandler.Setup(m => m.DownloadString(It.IsAny<Uri>())).Throws<UriFormatException>();

            Assert.Throws<UriFormatException>(() => _webClientHandler.DownloadString(new Uri("InvalidUri")));
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
