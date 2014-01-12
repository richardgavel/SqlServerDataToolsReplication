using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Source
{
    public class Columns
    {
        [XmlElement("Include")]
        public List<Column> Inclusions { get; set; }

        [XmlElement("Exclude")]
        public List<Column> Exclusions { get; set; }
    }
}
