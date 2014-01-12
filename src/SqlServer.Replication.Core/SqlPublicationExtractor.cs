using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Creates a DataSchemaModel object representing all the publications defined in a SQL Server database
    /// </summary>
    public class SqlPublicationExtractor
    {
        public DataSchemaModel Extract(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return Extract(connection);
            }
        }

        public DataSchemaModel Extract(SqlConnection connection)
        {
            var result = new List<Element>();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT pubid FROM dbo.syspublications";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(ExtractPublication(connection, reader.GetInt32("pubid").Value));
                }
            }

            return new DataSchemaModel
            {
                Model = new Model.Compiled.Model
                {
                    Elements = result
                }
            };
        }

        private static Element ExtractPublication(SqlConnection connection, int id)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT pubid, name, status, repl_freq, sync_method, description, immediate_sync, enabled_for_internet, " +
                                    "allow_push, allow_pull, allow_anonymous, replicate_ddl, backward_comp_level, snapshot_in_defaultfolder, alt_snapshot_folder, " +
                                    "independent_agent, allow_sync_tran, autogen_sync_procs, retention, allow_queued_tran, snapshot_in_defaultfolder, pre_snapshot_script, " +
                                    "post_snapshot_script, ftp_address, compress_snapshot, ftp_port, ftp_subdirectory, ftp_login, ftp_password, allow_dts, allow_subscription_copy, " +
                                    "conflict_policy, centralized_conflicts, conflict_retention, queue_type, allow_initialize_from_backup " +
                                    "" +
                                    "FROM dbo.syspublications WHERE pubid = " + id;

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Element
                    {
                        Type = "SqlPublication",
                        Id = reader.GetInt32("pubid").Value,
                        Name = reader.GetString("name"),
                        Properties = new List<Property>
                                {
                                    new Property { Name = "AllowAnonymous", Value = reader.GetBoolean("allow_dts") },
                                    new Property { Name = "AllowDts", Value = reader.GetBoolean("allow_anonymous") },
                                    new Property { Name = "AllowInitializeFromBackup", Value = reader.GetBoolean("allow_initialize_from_backup") },
                                    new Property { Name = "AllowPull", Value = reader.GetBoolean("allow_pull") },
                                    new Property { Name = "AllowPush", Value = reader.GetBoolean("allow_push") },
                                    new Property { Name = "AllowQueuedTransactions", Value = reader.GetBoolean("allow_queued_tran") },
                                    new Property { Name = "AllowSubscriptionCopy", Value = reader.GetBoolean("allow_subscription_copy") },
                                    new Property { Name = "AllowSynchronousTransactions", Value = reader.GetBoolean("allow_sync_tran") },
                                    new Property { Name = "AlternateSnapshotFolder", Value = reader.GetString("alt_snapshot_folder") },
                                    new Property { Name = "AutoGenerateSynchronizationStoredProcedures", Value = reader.GetBoolean("autogen_sync_procs") },
                                    new Property { Name = "BackwardsCompatabilityLevel", Value = reader.GetEnum<SqlPublicationBackwardsCompatibilityLevel>("backward_comp_level") },
                                    new Property { Name = "CompressSnapshot", Value = reader.GetBoolean("compress_snapshot") },
                                    new Property { Name = "CentralizedConflicts", Value = reader.GetBoolean("centralized_conflicts") },
                                    new Property { Name = "ConflictPolicy", Value = string.Empty },
                                    new Property { Name = "ConflictRetention", Value = reader.GetInt32("conflict_retention").ToString() },
                                    new Property { Name = "Description", Value = reader.GetString("description") },
                                    new Property { Name = "EnabledForInternet", Value = reader.GetBoolean("enabled_for_internet") },
                                    new Property { Name = "FtpAddress", Value = reader.GetString("ftp_address") },
                                    new Property { Name = "FtpLogin", Value = reader.GetString("ftp_login") },
                                    new Property { Name = "FtpPassword", Value = reader.GetString("ftp_password") },
                                    new Property { Name = "FtpPort", Value = reader.GetInt32("ftp_port").ToString() },
                                    new Property { Name = "FtpSubdirectory", Value = reader.GetString("ftp_subdirectory") },
                                    new Property { Name = "ImmediateSync", Value = reader.GetBoolean("immediate_sync") },
                                    new Property { Name = "IndependentAgent", Value = reader.GetBoolean("independent_agent") },
                                    new Property { Name = "IsSnapshotInDefaultFolder", Value = reader.GetBoolean("snapshot_in_defaultfolder") },
                                    new Property { Name = "PostSnapshotScript", Value = reader.GetString("post_snapshot_script") },
                                    new Property { Name = "PreSnapshotScript", Value = reader.GetString("pre_snapshot_script") },
                                    new Property { Name = "QueueType", Value = string.Empty },
                                    new Property { Name = "ReplicateDDL", Value = TranslateBoolean(reader.GetInt32(reader.GetOrdinal("replicate_ddl"))) },
                                    new Property { Name = "ReplicationFrequency", Value = reader.GetEnum<SqlPublicationReplicationFrequency>("repl_freq") },
                                    new Property { Name = "RetentionPeriod", Value = reader.GetInt32("retention").ToString() },
                                    new Property { Name = "SnapshotInDefaultFolder", Value = reader.GetBoolean("snapshot_in_defaultfolder") },
                                    new Property { Name = "Status", Value = reader.GetEnum<SqlPublicationStatus>("status") },
                                    new Property { Name = "SynchronizationMethod", Value = reader.GetEnum<SqlPublicationSynchronizationMethod>("sync_method") },
                                },
                        Relationships = new List<Relationship>
                                {
                                    new Relationship { Name = "Articles", Entries = ExtractArticles(connection, reader.GetInt32(reader.GetOrdinal("pubid"))) }                                    
                                }
                    };
                }
            }

            return null;
        }

        private static List<Entry> ExtractArticles(SqlConnection connection, int id)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT sysarticles.artid, sysarticles.name, schemas.name AS schema_name, objects.name AS object_name, sysarticles.status, creation_script, " +
                                    " del_cmd, filter, filter_clause, ins_cmd, upd_cmd, description, dest_owner, dest_table, status, CAST(schema_option AS BIGINT) AS schema_option " +
                                    "FROM dbo.sysarticles " +
                                    "INNER JOIN sys.objects " +
                                    "   ON sysarticles.objid = objects.object_id " +
                                    "INNER JOIN sys.schemas " +
                                    "   ON objects.schema_id = schemas.schema_id " +
                                    "WHERE pubid = " + id + " " +
                                    "ORDER BY objects.name";

            var reader = command.ExecuteReader();

            var result = new List<Entry>();

            while (reader.Read())
            {
                var entry = new Entry
                {
                    Element = new Element
                    {
                        Type = "SqlPublicationArticle",
                        Id = reader.GetInt32("artid").Value,
                        Name = string.Format("[{0}].[{1}]", reader.GetString("schema_name"), reader.GetString("object_name")),
                        Properties = new List<Property>
                                    {
                                        new Property { Name = "CreationScript", Value = reader.GetString("creation_script") },
                                        new Property { Name = "DeleteCommand", Value = reader.GetString("del_cmd") },
                                        new Property { Name = "Description", Value = reader.GetString("description") },
                                        new Property { Name = "DestinationOwner", Value = reader.GetString("dest_owner") },
                                        new Property { Name = "DestinationTable", Value = reader.GetString("dest_table") },
                                        new Property { Name = "Filter", Value = string.Empty },
                                        new Property { Name = "FilterClause", Value = string.Empty },
                                        new Property { Name = "InsertCommand", Value = reader.GetString("ins_cmd") },
                                        new Property { Name = "Name", Value = reader.GetString("name") },
                                        new Property { Name = "StatusActive", Value = TranslateStatusFlag(reader.GetByte(reader.GetOrdinal("status")), 1) },
                                        new Property { Name = "StatusIncludeColumnNameInInsert", Value = TranslateStatusFlag(reader.GetByte(reader.GetOrdinal("status")), 8) },
                                        new Property { Name = "StatusUseParameterizedStatements", Value = TranslateStatusFlag(reader.GetByte(reader.GetOrdinal("status")), 16) },
                                        new Property { Name = "UpdateCommand", Value = reader.GetString("upd_cmd") }
                                    },
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
                                                                        Name = string.Format("[{0}].[{1}]", reader.GetString("schema_name"), reader.GetString("object_name"))
                                                                    }
                                                            }
                                                    }
                                            },
                                        new Relationship { Name = "Columns", Entries = ExtractColumns(connection, reader.GetInt32("artid").Value) }
                                    }
                    }
                };

                entry.Element.Properties = entry.Element.Properties.Add(ExtractSchemaOptionsProperties(reader.GetInt64("schema_option"))).OrderBy(x => x.Name).ToList();

                result.Add(entry);
            }

            return result;
        }

        /// <summary>
        /// Helper method to populate all "SchemaOption*" properties based on the single bitmask and the enumeration. Avoids hand coding for each option
        /// </summary>
        private static IEnumerable<Property> ExtractSchemaOptionsProperties(long schemaOptions)
        {
            return Enum.GetNames(typeof (SqlPublicationSchemaOptions))
                       .Select(x => new Property
                           {
                               Name = "SchemaOption" + x,
                               Value = TranslateStatusFlag(schemaOptions, (long)Enum.Parse(typeof (SqlPublicationSchemaOptions), x))
                           });
        }

        private static List<Entry> ExtractColumns(SqlConnection connection, int articleId)
        {
            var command = connection.CreateCommand();
            command.CommandText = "SELECT sysarticles.artid, schemas.name AS schema_name, tables.name AS table_name, columns.name AS column_name " +
                                  "FROM sysarticlecolumns " +
                                  "INNER JOIN sysarticles " +
                                  "  ON sysarticlecolumns.artid = sysarticles.artid " +
                                  "  AND sysarticles.artid = " + articleId + " " +
                                  "INNER JOIN sys.tables " +
                                  "  ON sysarticles.objid = tables.object_id " +
                                  "INNER JOIN sys.schemas " +
                                  "  ON tables.schema_id = schemas.schema_id " +
                                  "INNER JOIN sys.columns " +
                                  "  ON sysarticles.objid = columns.object_id " +
                                  "  AND sysarticlecolumns.colid = columns.column_id " +
                                  "ORDER BY columns.name";

            var reader = command.ExecuteReader();

            var result = new List<Entry>();

            while (reader.Read())
            {
                var entry = new Entry
                {
                    References = new References
                    {
                        Name = string.Format("[{0}].[{1}].[{2}]", reader.GetString("schema_name"), reader.GetString("table_name"), reader.GetString("column_name"))
                    }
                };

                result.Add(entry);
            }

            return result;
        }

        private static string TranslateStatusFlag(long value, long mask)
        {
            return (value & mask) > 0 ? "True" : "False";
        }

        private static string TranslateBoolean(int value)
        {
            switch (value)
            {
                case 1:
                    return "True";
                case 0:
                    return "False";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

