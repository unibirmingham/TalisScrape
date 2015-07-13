using System.Runtime.Serialization;

namespace TalisScraper.Objects.JsonMaps
{
    [DataContract]
    public class Resources
    {
        [DataMember(Name = "root")]
        public ResourceItems Items { get; set; }
    }
}
