using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Compiled
{
    public class Relationship
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Entry")]
        public List<Entry> Entries { get; set; } 
    }
}
