using System;

namespace SqlServer.Replication.Core
{
    [Flags]
    internal enum SqlPublicationSchemaOptions : long
    {
        ConvertSpatialDataTypesToVarbinaryMax = 0x8000000000,
        ConvertHierarchyIdDataTypeToVarbinaryMax = 0x2000000000,
        ConvertLargeClrUserDefinedTypesToVarbinaryMax = 0x1000000000,
        ConvertLargeDataTypesToSql2000CompliantTypes = 0x20000000,
        ConvertSql2008DateTimeDataTypesToEarlierVersionCompliantTypes = 0x200000000,
        ConvertTimestampToBinary = 0x08,
        ConvertUserDefinedTypesToBaseTypes = 0x20,
        ConvertXmlToNText = 0x10000000,
        CopyCheckConstraints = 0x400,
        CopyCheckConstraintsAsNotForReplication = 0x10000,
        CopyClusteredIndex = 0x10,
        CopyCollation = 0x1000,
        CopyCompressionOptions = 0x400000000,
        CopyDefaults = 0x800,
        CopyDefaultBindings = 0x400000,
        CopyExtendedProperties = 0x2000,
        CopyFilegroupsForPartitionedObjects = 0x40000,
        CopyFileStreamAttributes = 0x100000000,
        CopyFilteredIndexes = 0x4000000000,
        CopyForeignKeyConstraints = 0x200,
        CopyForeignKeyConstraintsAsNotForReplication = 0x20000,
        CopyFullTextIndexes = 0x1000000,
        CopyIdentityColumnsAsIdentity = 0x04,
        CopyIndexesOnSpatialDataTypes = 0x10000000000,
        CopyNonClusteredIndexes = 0x40,
        CopyPartitionSchemesForPartitionedIndexes = 0x100000,
        CopyPartitionSchemesForPartitionedTables = 0x80000,
        CopyPermissions = 0x40000000,
        CopyPrimaryKey = 0x80,
        CopyRuleBindings = 0x800000,
        CopySparseColumnAttributes = 0x20000000000,
        CopyTableStatistics = 0x200000,
        CopyTimestampColumnsAsTimestamp = 0x08,
        CopyUniqueConstraints = 0x4000,
        CopyUserTriggers = 0x100,
        CopyXmlIndexes = 0x4000000,
        DoNotReplicateXmlSchemaCollections = 0x4000000,
        DropDependenciesOnObjectsNotInReplication = 0x80000000,
        CreateSchemasAtSubscriber = 0x8000000,
        GenerateObjectCreationScripts = 0x01,
        GenerateChangePropagationStoredProcedures = 0x02,
        OptionInvalidForSqlServer2005Publishers = 0x8000,        // Kept in for completeness (in sp_addarticle help),
        StoreFilestreamDataOnSeparateFilegroup = 0x800000000
    }
}
