using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using SqlServer.Replication.Core;

namespace SqlServer.Replication.UnitTests
{
    [TestFixture]
    public class DacpacAugmentation
    {
        [Test]
        public void Execute()
        {
            using (var connection = new SqlConnection(@""))
            {
                connection.Open();

                var augmentor = new SqlPublicationDacpacAugmentor();
                //augmentor.Augment(new FileInfo(@""), connection);
            }
        }
    }
}
