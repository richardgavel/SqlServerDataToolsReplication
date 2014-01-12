using System.Collections.Generic;
using System.Xml.Serialization;

namespace SqlServer.Replication.Model.Source
{
    public class Table
    {
        [XmlAttribute]
        public string Name { get; set; }

        public Columns Columns { get; set; }
    }
}
