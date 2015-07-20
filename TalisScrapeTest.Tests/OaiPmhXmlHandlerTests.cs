using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;
using TalisScraper.Objects;
using TalisScraper.Objects.Handlers;
using TalisScraper.Objects.JsonMaps;

namespace TalisScrapeTest.Tests
{    /*
    * ################################################
    * ############# NAMING CONVENTION ################
    * ################################################
    * # MethodName_StateUnderTest_ ExpectedBehaviour #
    * ################################################
    */

    [TestFixture]
    public class OaiPmhXmlHandlerTests
    {
        private OaiPmhXmlHandler _exportHandler;
        private ICollection<ReadingList> _readingListCollection;
        
        [SetUp]
        public void SetUp()
        {
            _exportHandler = new OaiPmhXmlHandler();
            _readingListCollection = new Collection<ReadingList> { new ReadingList { Uri = "SomeUri", Books = new List<Book>(), ListInfo = new NavItem()}};
        }

        [Test]
        public void Export_NullReadingListPassedIn_ReturnsEmptyString()
        {
            Assert.IsNull(_exportHandler.Export(null));
        }

        [Test]
        public void Export_EmptyReadingListPassedIn_ReturnsEmptyString()
        {
            Assert.IsNull(_exportHandler.Export(new Collection<ReadingList>()));
        }


        [Test]
        [Ignore]//TODO: We need to flsh this out a bit! Is there an easy way of determining XML structure from test Collection<ReadingList> ?? 
        public void Export_PopulatedReadingListPassedIn_ReturnsCorrectXmlDocumentAsString()
        {

            var result = _exportHandler.Export(_readingListCollection);


            StringAssert.AreEqualIgnoringCase("", result);
        }


        [TearDown]
        public void TearDown()
        {
            _exportHandler = null;
        }
    }
}
