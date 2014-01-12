using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Source
{
    public class Column
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}
