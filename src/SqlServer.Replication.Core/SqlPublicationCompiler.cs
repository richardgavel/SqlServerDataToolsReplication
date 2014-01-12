using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.SqlServer.Dac.Model;
using SqlServer.Replication.Model.Compiled;
using Source = SqlServer.Replication.Model.Source;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Creates a DataSchemaModel compiled output based upon a list of database publications and the corresponding schema model
    /// </summary>
    public class SqlPublicationCompiler
    {
        public DataSchemaModel Compile(string projectFilePath, string dacpacPath)
        {
            Debug.Assert(projectFilePath != null, "projectFilePath != null");

            var projectReplicationsDirectory = Path.Combine(Path.GetDirectoryName(projectFilePath) ?? string.Empty, "Replications");

            if (!Directory.Exists(projectReplicationsDirectory))
                return new DataSchemaModel { Model = new Model.Compiled.Model() };

            var sourceSerializer = new XmlSerializer(typeof(Source.Publication));

            var publications = Directory.EnumerateFiles(projectReplicationsDirectory, "*.xml")
                                        .Where(x => Path.GetFileName(x) != "Replication.DefaultSettings.xml")
                                        .Select(x => (Source.Publication) sourceSerializer.Deserialize(new FileStream(x, FileMode.Open)))
                                        .ToList();

            var sqlModel = new TSqlModel(dacpacPath);
            return Compile(publications, sqlModel);
        }

        public DataSchemaModel Compile(List<Source.Publication> publications, TSqlModel sqlModel)
        {
            return new DataSchemaModel
            {
                Model = new Model.Compiled.Model
                {
                    Elements = publications.Select(x => new Element
                    {
                        Type = "SqlPublication",
                        Name = x.Name,
                        Properties = new List<Property>
                            {
                                new Property { Name = "AllowAnonymous", Value = "True" },
                                new Property { Name = "AllowPull", Value = "True" },
                                new Property { Name = "AllowPush", Value = "True" },
                                new Property { Name = "AllowSynchronousTransactions", Value = "False" },
                                new Property { Name = "AutoGenerateSynchronizationStoredProcedures", Value = string.Empty },
                                new Property { Name = "CompressSnapshot", Value = "False" },
                                new Property { Name = "Description", Value = x.Name },
                                new Property { Name = "EnabledForInternet", Value = "False" },
                                new Property { Name = "ImmediateSync", Value = "True" },
                                new Property { Name = "IndependentAgent", Value = "False" },
                                new Property { Name = "PostSnapshotScript", Value = string.Empty },
                                new Property { Name = "PreSnapshotScript", Value = string.Empty },
                                new Property { Name = "ReplicateDDL", Value = "True" },
                                new Property { Name = "ReplicationFrequency", Value = "Continuous" },
                                new Property { Name = "Rentention", Value = "0" },
                                new Property { Name = "StatusActive", Value = "True" },
                                new Property { Name = "SnapshotInDefaultFolder", Value = "True" },
                                new Property { Name = "SynchronizationMethod", Value = "ConcurrentCharacter" }
                            },
                        Relationships = new List<Relationship>
                            {
                                new Relationship { Name = "Articles", Entries = CompileArticles(x.Tables, sqlModel) }
                            }
                    }).ToList()
                }
            };
        }

        private static List<Entry> CompileArticles(IEnumerable<Source.Table> tables, TSqlModel model)
        {
            var result = new List<Entry>();

            foreach (var table in tables)
            {
                var tableNameParts = table.Name.Split('.');
                if (tableNameParts.Length == 1)
                    tableNameParts = new[] { "dbo", table.Name };

                var sqlTable = model.GetObject(ModelSchema.Table, new ObjectIdentifier(tableNameParts), DacQueryScopes.UserDefined);

                if (sqlTable == null)
                    throw new Exception(string.Format("Unable to find table in model: {0}", table.Name));

                if ((table.Columns != null) && (table.Columns.Exclusions != null) && (table.Columns.Inclusions != null) && (table.Columns.Exclusions.Any()) && (table.Columns.Inclusions.Any()))
                    throw new Exception("Can not have both inclusions and exclusions for same table");

                var sqlColumns = sqlTable.GetChildren().Where(x => x.ObjectType == ModelSchema.Column).ToList();

                if (table.Columns != null)
                {
                    if ((table.Columns.Inclusions != null) && (table.Columns.Inclusions.Any()))
                    {
                        // TODO: Add checking for bad column names
                        sqlColumns.RemoveAll(sqlColumn => table.Columns.Inclusions.All(inclusion => sqlColumn.Name.Parts.Last() != inclusion.Name));
                    }

                    if ((table.Columns.Exclusions != null) && (table.Columns.Exclusions.Any()))
                    {
                        // TODO: Add checking for bad column names
                        sqlColumns.RemoveAll(sqlColumn => table.Columns.Exclusions.Any(exclusion => sqlColumn.Name.Parts.Last() == exclusion.Name));
                    }
                }

                result.Add(new Entry
                    {
                        Element = new Element
                            {
                                Name = sqlTable.Name.ToString(),
                                Properties = GenerateArticleProperties(sqlTable),
                                Relationships = new List<Relationship>
                                    {
                                        new Relationship
                                            {
                                                Name = "Table",
                                                Entries = new List<Entry>
                                                    {
                                                        new Entry
                                                            {
                                                                References = new References
                                                                    {
                                                                        Name = sqlTable.Name.ToString()
                                                                    }
                                                            }
                                                    }
                                            },
                                        new Relationship
                                            {
                                                Name = "Columns",
                                                Entries = sqlColumns.Select(sqlColumn => new Entry
                                                    {
                                                        References = new References
                                                            {
                                                                Name = sqlColumn.Name.ToString()
                                                            }
                                                    }).ToList()
                                            }
                                    }
                            }
                    });
            }

            return result;
        }

        /// <summary>
        /// Helper method to generate properties for an article. Separated out to permit creation of schema options by only specifying by hand
        /// those options which are true and defaulting the rest to false. This was not possible using collection initializers.
        /// </summary>
        private static List<Property> GenerateArticleProperties(TSqlObject sqlTable)
        {
            SqlPublicationSchemaOptions[] defaultActiveSchemaOptions =
            {
                SqlPublicationSchemaOptions.ConvertTimestampToBinary,
                SqlPublicationSchemaOptions.CopyClusteredIndex,
                SqlPublicationSchemaOptions.CopyPrimaryKey,
                SqlPublicationSchemaOptions.CreateSchemasAtSubscriber
            };

            var result = new List<Property>();

            result.AddRange(new [] {
                new Property { Name = "Description", Value = string.Empty},
                new Property { Name = "DestinationOwner", Value = sqlTable.Name.Parts[0] },
                new Property { Name = "DestinationTable", Value = sqlTable.Name.Parts[1] },
                new Property { Name = "Name", Value = sqlTable.Name.Parts[1] }
            });

            foreach (var schemaOptionName in Enum.GetNames(typeof(SqlPublicationSchemaOptions)))
            {
                var value = defaultActiveSchemaOptions.Contains(schemaOptionName.AsEnum<SqlPublicationSchemaOptions>()) ? "True" : "False";
                result.Add(new Property { Name = "SchemaOption" + schemaOptionName, Value = value });
            }

            return result;
        }
    }
}
