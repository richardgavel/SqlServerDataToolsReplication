using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Compiled
{
    public class Property
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Value { get; set; }
    }
}
