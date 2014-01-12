using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Source
{
    public class Publication
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Table")]
        public List<Table> Tables { get; set; }
    }
}
