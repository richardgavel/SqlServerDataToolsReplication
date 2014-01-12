using System.Collections.Generic;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core
{
    public class SqlPublicationArticleChangeDefinition
    {
        public IList<string> PropertiesEqual { get; set; }

        public IList<string> PropertiesModified { get; set; }

        public List<Entry> ColumnsAdded { get; set; }

        public List<Entry> ColumnsDropped { get; set; } 
    }
}
