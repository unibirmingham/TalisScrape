using NUnit.Framework;
using TalisScraper;
using TalisScraper.Objects;

namespace TalisScrapeTest.Tests
{
    interface IScraperTest : IScraper
    {
        string Json { get; set; }
    }

    class ScraperTest : IScraperTest
    {
        public string Json { get; set; }
        public new string FetchJson(string name = "")
        {
            return Json;
        }

        Base IScraper.FetchItems(string items)
        {
            throw new System.NotImplementedException();
        }

        public T FetchItems<T>(string items)
        {
            throw new System.NotImplementedException();
        }

        public Items FetchItems(string name)
        {
            var json = FetchJson();

            if (string.IsNullOrEmpty(json))
                return null;

            //var jReader = new JsonFx.Json.JsonReader();
            //JOb
            var schools = json.FromJson<Items>(); //JsonConvert.DeserializeObject<Schools>(json);//.Read<Schools>(json);//json.FromJson<Schools>();

            return schools;
        }
    }
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
        private IScraperTest _scraper;

        private string genericJson =
            "{\"http://demo.talisaspire.com/\": {\"http://purl.org/vocab/aiiso/schema#name\": [{\"value\": \"Broadminster\",\"type\": \"literal\"}], \"http://purl.org/vocab/aiiso/schema#organizationalUnit\": [{\"value\": \"http://demo.talisaspire.com/schools/bs02\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/div001\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/emps01\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/gold01\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/hum02\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/les02\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/sch001\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/gg100\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/cc101\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/schbed\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/art100\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/schools/inv\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/institutes/mie1\",\"type\": \"uri\"}],\"http://www.w3.org/1999/02/22-rdf-syntax-ns#type\": [{\"value\": \"http://purl.org/vocab/aiiso/schema#Institution\",\"type\": \"uri\"},{\"value\": \"http://purl.org/vocab/aiiso/schema#organizationalUnit\",\"type\": \"uri\"}],\"http://purl.org/vocab/aiiso/schema#knowledgeGrouping\": [{\"value\": \"http://demo.talisaspire.com/subjects/tadc101\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/courses/hum325\",\"type\": \"uri\"},{\"value\": \"http://demo.talisaspire.com/courses/chm174\",\"type\": \"uri\"}],\"http://purl.org/vocab/aiiso/schema#code\": [{\"value\": \"INSTITUTION\",\"type\": \"literal\"}]}}";   

        [SetUp]
        public void SetUp()
        {
            _scraper = new ScraperTest();
        }

        [Test]
        public void FetchJson_InjhectedJsonStringReturned_ReturnsTrue()
        {
            _scraper.Json = genericJson;

            Assert.AreEqual(_scraper.FetchJson(), genericJson);
        }

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
        }


        [TearDown]
        public void TearDown()
        {
            _scraper = null;
        }
    }
}
