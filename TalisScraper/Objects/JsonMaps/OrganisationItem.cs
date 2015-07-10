using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TalisScraper.Objects.JsonMaps
{
    public class OrganisationItem
    {
        [DataMember(Name = "http://xmlns.com/foaf/0.1/name")]
        public IEnumerable<Element> Publishers { get; set; }
    }
}
