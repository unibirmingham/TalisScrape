using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Extensions;
using TalisScraper.Interfaces;

namespace TalisScraper.Objects.Handlers
{
    public class OaiPmhXmlHandler : IExportHandler
    {
        public string Export(IEnumerable<ReadingList> readingLists)
        {
            if (!readingLists.HasContent())
                return null;

            var xmlDoc = new ListRecords();
            var recordList = new Collection<recordType>();

            foreach (var list in readingLists)
            {
                var record = new recordType
                {
                    header = new headerType
                    {
                        identifier = list.Uri
                    }
                };

                recordList.Add(record);
            }

            xmlDoc.record = recordList.ToArray();

            var builder = new StringBuilder();
            var settings = new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true};
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");


            var serializer = new XmlSerializer(typeof(ListRecords));

            using (var writer = XmlWriter.Create(builder, settings))
            {
                serializer.Serialize(writer, xmlDoc, ns);
            }


            return builder.ToString();
        }
    }
}
