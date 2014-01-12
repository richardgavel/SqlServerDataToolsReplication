using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.SqlServer.Dac.Deployment;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core.Deployment
{
    /// <summary>
    /// Deployment step that generates the SQL necessary to create a new publication based upon a compiled model
    /// </summary>
    public class CreatePublicationStep : DeploymentStep
    {
        public CreatePublicationStep(Element sourceElement)
        {
            SourceElement = sourceElement;
        }

        public Element SourceElement { get; private set; }

        public override IList<string> GenerateTSQL()
        {
            var result = new List<string>();

            result.Add(string.Format("sp_addpublication @publication = '{0}', @sync_method = {1}, @repl_freq = {2}, @description = '{3}', @status = {4}, " +
                        "@immediate_sync = '{5}', @enabled_for_internet = '{6}', @allow_push = '{7}', @allow_pull = '{8}', @allow_anonymous = '{9}', " +
                        "@replicate_ddl = {10};",
                SourceElement.Name,
                (int)Enum.Parse(typeof(SqlPublicationSynchronizationMethod), SourceElement.Properties.Single(x => x.Name == "SynchronizationMethod").Value),
                (int)Enum.Parse(typeof(SqlPublicationReplicationFrequency), SourceElement.Properties.Single(x => x.Name == "ReplicationFrequency").Value),
                SourceElement.Properties.Single(x => x.Name == "Description").Value,
                (int)Enum.Parse(typeof(SqlPublicationStatus), SourceElement.Properties.Single(x => x.Name == "Status").Value),
                SourceElement.Properties.Single(x => x.Name == "ImmediateSync").Value.ToLower(),
                SourceElement.Properties.Single(x => x.Name == "EnabledForInternet").Value.ToLower(),
                SourceElement.Properties.Single(x => x.Name == "AllowPush").Value.ToLower(),
                SourceElement.Properties.Single(x => x.Name == "AllowPull").Value.ToLower(),
                SourceElement.Properties.Single(x => x.Name == "AllowAnonymous").Value.ToLower(),
                (int)Enum.Parse(typeof(SqlPublicationReplicateDdl), SourceElement.Properties.Single(x => x.Name == "ReplicateDDL").Value)));

            result.Add(string.Empty);

            foreach (var article in SourceElement.Relationships.Single(x => x.Name == "Articles").Entries.Select(x => x.Element))
            {
                var tableParts = article.Relationships.Single(x => x.Name == "Table").Entries.First().References.Name.Split('.');

                result.Add(string.Format("sp_addarticle @publication = '{0}', @article = '{1}', " +
                            "@destination_table = '{2}', @description = '{3}', @schema_option = {4}, @destination_owner = '{5}', @source_owner = '{6}', @source_object = '{7}';",
                    SourceElement.Name,
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
                        SourceElement.Name, article.Properties.Single(x => x.Name == "Name").Value, column.Name.Split('.').Last()));
                }

                result.Add(string.Empty);
            }

            return result;
        }
    }

    internal static class CreatePublicationStepExtensions
    {
        internal static string GetSchemaOptions(this Element article)
        {
            var result = 0L;

            foreach (var schemaOptionName in Enum.GetNames(typeof (SqlPublicationSchemaOptions)))
            {
                if (article.Properties.Single(x => x.Name == "SchemaOption" + schemaOptionName).Value == "True")
                    result = result | (long) Enum.Parse(typeof (SqlPublicationSchemaOptions), schemaOptionName);
            }

            return result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
