using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Compiled
{
    public class Model
    {
        [XmlElement("Element")]
        public List<Element> Elements { get; set; } 
    }
}
