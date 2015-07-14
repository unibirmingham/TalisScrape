using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TalisScraper.Objects.JsonMaps
{
    [DataContract]
    public class ResourceItems
    {
        [DataMember(Name = "http://purl.org/dc/elements/1.1/subject")]
        public IEnumerable<Element> Subject { get; set; }

        [DataMember(Name = "http://purl.org/dc/terms/date")]
        public IEnumerable<Element> Date { get; set; }

        [DataMember(Name = "http://purl.org/dc/terms/title")]
        public IEnumerable<Element> Title { get; set; }

        [DataMember(Name = "http://purl.org/ontology/bibo/isbn10")]
        public IEnumerable<Element> Isbn10 { get; set; }

        [DataMember(Name = "http://purl.org/ontology/bibo/isbn13")]
        public IEnumerable<Element> Isbn13 { get; set; }

        [DataMember(Name = "http://rdvocab.info/elements/placeOfPublication")]
        public IEnumerable<Element> PlaceOfPublication { get; set; }

        [DataMember(Name = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type")]
        public IEnumerable<Element> Type { get; set; }

        [DataMember(Name = "http://purl.org/dc/terms/source")]
        public IEnumerable<Element> Source { get; set; }


        //public IEnumerable<Element> Authors { get; set; } 
    }
}
