using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Dac.Deployment;

namespace SqlServer.Replication.Core.Deployment
{
    /// <summary>
    /// Deployment step that generates the SQL necessary to alter a publication based upon the differences found
    /// between the current state and desired end state of that publication
    /// </summary>
    public class AlterPublicationStep : DeploymentStep
    {
        public AlterPublicationStep(SqlPublicationCompareResult compareResult)
        {
            CompareResult = compareResult;
        }

        public SqlPublicationCompareResult CompareResult { get; private set; }

        public override IList<string> GenerateTSQL()
        {
            var result = new List<string>();

            var publicationName = CompareResult.SourceElement.Name;

            foreach (var article in CompareResult.ElementsToAdd)
            {
                var tableParts = article.Relationships.Single(x => x.Name == "Table").Entries.First().References.Name.Split('.');

                result.Add(string.Format("sp_addarticle @publication = '{0}', @article = '{1}', " +
                            "@destination_table = '{2}', @description = '{3}', @schema_option = {4}, @destination_owner = '{5}', @source_owner = '{6}', @source_object = '{7}';",
                    publicationName,
                    article.Properties.Single(x => x.Name == "Name").Value,
                    article.Properties.Single(x => x.Name == "DestinationTable").Value,
                    article.Properties.Single(x => x.Name == "Description").Value,
                    article.GetSchemaOptions(),
                    article.Properties.Single(x => x.Name == "DestinationOwner").Value,
                    tableParts[0],
                    tableParts[1]));

                foreach (var column in article.Relationships.Single(x => x.Name == "Columns").Entries.Select(x => x.References))
                {
                    result.Add(string.Format("sp_articlecolumn @publication = '{0}', @article = '{1}', @column = '{2}', @operation = 'add';",
                        publicationName, article.Properties.Single(x => x.Name == "Name").Value, column.Name.Split('.').Last()));
                }

                result.Add(string.Empty);
            }

            foreach (var article in CompareResult.ElementsToDrop)
            {
                result.Add(string.Format("sp_droparticle @publication = '{0}', @article = '{1}'", publicationName, article.Properties.Single(x => x.Name == "Name").Value));
                result.Add(string.Empty);
            }

            foreach (var article in CompareResult.ElementsChanged)
            {
                foreach (var column in article.Value.ColumnsAdded)
                {
                    result.Add(string.Format("sp_articlecolumn @publication = '{0}', @article = '{1}', @column = '{2}', @operation = 'add';",
                        publicationName, article.Key.Properties.Single(x => x.Name == "Name").Value, column.References.Name.Split('.').Last()));
                }

                foreach (var column in article.Value.ColumnsDropped)
                {
                    result.Add(string.Format("sp_articlecolumn @publication = '{0}', @article = '{1}', @column = '{2}', @operation = 'drop';",
                        publicationName, article.Key.Properties.Single(x => x.Name == "Name").Value, column.References.Name.Split('.').Last()));
                }
            }

            return result;
        }
    }
}
