using System.Runtime.Serialization;

namespace TalisScraper.Objects
{
    [DataContract]
    public class Base
    {
        [DataMember(Name = "root")]
        public Items Items { get; set; }
    }
}
