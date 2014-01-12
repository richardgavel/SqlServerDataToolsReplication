using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Compiled
{
    public class References
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}
