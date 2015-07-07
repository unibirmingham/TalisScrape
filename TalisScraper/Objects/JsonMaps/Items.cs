using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TalisScraper.Objects.JsonMaps
{
    [DataContract]
    public class Items
    {
        [DataMember(Name = "http://purl.org/vocab/aiiso/schema#code")]
        public IEnumerable<Element> Code { get; set; }

        [DataMember(Name = "http://purl.org/vocab/aiiso/schema#description")]
        public IEnumerable<Element> Description { get; set; }

        [DataMember(Name = "http://purl.org/vocab/aiiso/schema#knowledgeGrouping")]
        public IEnumerable<Element> KnowledgeGrouping { get; set; }

        [DataMember(Name = "http://purl.org/vocab/aiiso/schema#name")]
        public IEnumerable<Element> Name { get; set; }

        [DataMember(Name = "http://purl.org/vocab/aiiso/schema#organizationalUnit")]
        public IEnumerable<Element> OrganizationalUnit { get; set; }

        [DataMember(Name = "http://purl.org/vocab/aiiso/schema#part_of")]
        public IEnumerable<Element> PartOf { get; set; }

        [DataMember(Name = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type")]
        public IEnumerable<Element> Type { get; set; }

        [DataMember(Name = "http://purl.org/vocab/resourcelist/schema#usesList")]
        public IEnumerable<Element> UsesList { get; set; }

        [DataMember(Name = "http://purl.org/vocab/resourcelist/schema#contains")]
        public IEnumerable<Element> Contains { get; set; }

        [DataMember(Name = "http://purl.org/vocab/resourcelist/schema#anticipatedStudentNumbers")]
        public IEnumerable<Element> AnticipatedStudentNumbers { get; set; }


    }

    public class Element
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
