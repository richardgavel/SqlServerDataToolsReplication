namespace SqlServer.Replication.Core
{
    internal enum SqlPublicationSynchronizationMethod
    {
        Native = 0,
        Character = 1,
        Concurrent = 2,
        ConcurrentCharacter = 3,
        DatabaseSnapshot = 4,
        DatabaseSnapshotCharacter = 5
    }
}
