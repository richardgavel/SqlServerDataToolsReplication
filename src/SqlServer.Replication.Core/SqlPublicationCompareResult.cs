using System.Collections.Generic;
using System.Collections.ObjectModel;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Output of comparing a single publication within a source and target
    /// </summary>
    public class SqlPublicationCompareResult
    {
        public Element SourceElement { get; set; }

        public Element TargetElement { get; set; }

        /// <summary>
        /// Publication articles that need to be added
        /// </summary>
        public ReadOnlyCollection<Element> ElementsToAdd { get; set; }

        /// <summary>
        /// Publication articles that need to be dropped
        /// </summary>
        public ReadOnlyCollection<Element> ElementsToDrop { get; set; }

        /// <summary>
        /// Publication articles that are identical
        /// </summary>
        public IDictionary<Element, Element> ElementsEqual { get; set; }

        /// <summary>
        /// Publication articles that exist in both source and target, but need to be modified in target
        /// </summary>
        public IDictionary<Element, SqlPublicationArticleChangeDefinition> ElementsChanged { get; set; }
    }
}
