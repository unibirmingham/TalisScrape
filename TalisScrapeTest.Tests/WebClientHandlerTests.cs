using System;
using System.Threading.Tasks;
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
    /*
    [TestFixture]
    public class WebClientHandlerTests
    {
      
        private IRequestHandler _requestHandler;
        private Mock<IRequestHandler> _moqReqHandler;

        [SetUp]
        public void SetUp()
        {
            _moqReqHandler = new Mock<IRequestHandler>();
        }

        [Test]
        public void FetchJson_ValidUriJsonObjectReturned_ReturnsExpectedStringJsonObject()
        {
            _moqReqHandler.Setup(m => m.FetchJson(new Uri("testUri"))).Returns("validJson");
            _requestHandler = _moqReqHandler.Object;

            Assert.AreEqual(_requestHandler.FetchJson(new Uri("testUri")), "validJson");
        }

  /*      [Test]
        public void FetchJsonAsync_ValidUriJsonObjectReturned_ReturnsExpectedStringJsonObject()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(new Uri("testUri"))).Returns(Task.FromResult("validJson"));
            _requestHandler = _moqReqHandler.Object;

            Assert.AreEqual(_requestHandler.FetchJson(new Uri("testUri")), "validJson");
        }*/
    /*
        [TearDown]
        public void TearDown()
        {
            _requestHandler = null;
            _moqReqHandler = null;
        }
    }*/
}
