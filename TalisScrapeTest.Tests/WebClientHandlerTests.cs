﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TalisScraper.Interfaces;

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
    public class WebClientHandlerTests
    {
      
        private IRequestHandler _requestHandler;
        private Mock<IRequestHandler> _moqReqHandler;

        private const string GenericJson = "{\"http://demo.talisaspire.com/\": {\"http://purl.org/vocab/aiiso/schema#name\": [{\"value\": \"Broadminster\",\"type\": \"literal\"}]}}";

        [SetUp]
        public void SetUp()
        {
          _moqReqHandler = new Mock<IRequestHandler>();
          _requestHandler = _moqReqHandler.Object;
        }

        #region Sync Tests

        [Test]
        public void FetchJson_ValidUriValidEndPoint_ReturnsExpectedJson()
        {
            _moqReqHandler.Setup(m => m.FetchJson("http://testuri.com")).Returns(GenericJson);
            Assert.AreEqual(_requestHandler.FetchJson("http://testuri.com"), GenericJson);
        }

        [Test]
        public void FetchJson_ValidUriInValidEndPoint_ThrowsWebException()
        {
            _moqReqHandler.Setup(m => m.FetchJson("http://invalidEndpoint.com")).Throws<WebException>();
            Assert.Throws<WebException>(() => _requestHandler.FetchJson("http://invalidEndpoint.com"));
        }

        [Test]
        public void FetchJson_ValidUriNoneJsonResponse_ThrowsInvalidDataException()
        {
            _moqReqHandler.Setup(m => m.FetchJson("http://noneJsonEndpoint.com")).Throws<InvalidDataException>();
            Assert.Throws<InvalidDataException>(() => _requestHandler.FetchJson("http://noneJsonEndpoint.com"));
        }

        [Test]
        public void FetchJson_InValidUri_ThrowsUriFormatException()
        {
            _moqReqHandler.Setup(m => m.FetchJson("InvalidUri")).Throws<UriFormatException>();
            Assert.Throws<UriFormatException>(() => _requestHandler.FetchJson("InvalidUri"));
        }

        #endregion

        #region Async Tests

        [Test]
        public async Task FetchJsonAsync_ValidUriValidEndPoint_ReturnsExpectedJson()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).ReturnsAsync(GenericJson);
            Assert.AreEqual(await _requestHandler.FetchJsonAsync("http://testuri.com").ConfigureAwait(false), GenericJson);
        }

        [Test]
        public async Task FetchJsonAsync_ValidUriInValidEndPoint_ThrowsWebException()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).Throws<WebException>();
            Assert.Throws<WebException>(async() => await _requestHandler.FetchJsonAsync("http://invalidEndpoint.com").ConfigureAwait(false));
        }

        [Test]
        public async Task FetchJsonAsync_ValidUriNoneJsonResponse_ThrowsInvalidDataException()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).Throws<InvalidDataException>();
            Assert.Throws<InvalidDataException>(async () => await _requestHandler.FetchJsonAsync("http://noneJsonEndpoint.com").ConfigureAwait(false));
        }

        [Test]
        public async Task FetchJsonAsync_InValidUri_ThrowsUriFormatException()
        {
            _moqReqHandler.Setup(m => m.FetchJsonAsync(It.IsAny<string>())).Throws<UriFormatException>();
            Assert.Throws<UriFormatException>(async () => await _requestHandler.FetchJsonAsync("InvalidUri").ConfigureAwait(false));
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            _requestHandler = null;
            _moqReqHandler = null;
        }
    }
}