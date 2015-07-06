using System.Runtime.Serialization;

namespace TalisScraper.Objects
{
    [DataContract]
    public class NavItem
    {
        [DataMember(Name = "root")]
        public Items Items { get; set; }
    }
}
