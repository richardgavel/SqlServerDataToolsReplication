using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Compiled
{
    public class Element
    {
        [XmlIgnore]
        public int Id { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Property")]
        public List<Property> Properties { get; set; }
        
        [XmlElement("Relationship")]
        public List<Relationship> Relationships { get; set; }
    }
}
