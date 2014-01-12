using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Compares two publication compiled models for changes
    /// </summary>
    public class SqlPublicationComparer
    {
        /// <summary>
        /// Compares two publications and returns result listing all identical aspects and differences
        /// </summary>
        /// <param name="source">The publication which represents the desired end state</param>
        /// <param name="target">The publication the changes are to be applied to</param>
        /// <returns>Results of the comparison</returns>
        public SqlPublicationCompareResult Compare(Element source, Element target)
        {
            var sourceArticles = source.Articles();
            var targetArticles = target.Articles();

            var articlesEqual = new Dictionary<Element, Element>();
            var articlesChanged = new Dictionary<Element, SqlPublicationArticleChangeDefinition>();

            foreach (var sourceArticle in sourceArticles.Intersect(targetArticles, x => x.TableName()))
            {
                var matchingTargetArticle = targetArticles.Single(targetArticle => targetArticle.TableName() == sourceArticle.TableName());

                var columnsToAdd = sourceArticle.Columns().Except(matchingTargetArticle.Columns(), x => x.ColumnName()).ToList();
                var columnsToRemove = matchingTargetArticle.Columns().Except(sourceArticle.Columns(), x => x.ColumnName()).ToList();

                if (!columnsToAdd.Any() && !columnsToRemove.Any())
                    articlesEqual.Add(sourceArticle, matchingTargetArticle);
                else
                {
                    var changeDefinition = new SqlPublicationArticleChangeDefinition
                        {
                            ColumnsAdded = columnsToAdd,
                            ColumnsDropped = columnsToRemove,
                            PropertiesEqual = new List<string>(),
                            PropertiesModified = new List<string>()
                        };

                    articlesChanged.Add(sourceArticle, changeDefinition);
                }
            }

            return new SqlPublicationCompareResult
            {
                SourceElement = source,
                ElementsToAdd = new ReadOnlyCollection<Element>(sourceArticles.Except(targetArticles, x => x.TableName()).ToList()),
                ElementsToDrop = new ReadOnlyCollection<Element>(targetArticles.Except(sourceArticles, x => x.TableName()).ToList()),
                ElementsEqual = articlesEqual,
                ElementsChanged = articlesChanged
            };
        }
    }

    /// <summary>
    /// Extension methods used in SqlPublicationComparer for readability
    /// </summary>
    internal static class SqlPublicationComparerExtensions
    {
        internal static List<Element> Articles(this Element publication)
        {
            return publication.Relationships.Single(x => x.Name == "Articles").Entries.Select(x => x.Element).ToList();
        }

        internal static string ColumnName(this Entry entry)
        {
            return entry.References.Name;
        }

        internal static IEnumerable<Entry> Columns(this Element article)
        {
            return article.Relationships.Single(x => x.Name == "Columns").Entries;
        }

        internal static string TableName(this Element article)
        {
            return article.Name;
        }
    }
}
