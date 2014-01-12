using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using SqlServer.Replication.Core;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.UnitTests
{
    [TestFixture]
    public class SqlPublicationExtractorTests
    {
        [Test]
        public void Execute()
        {
            using (var connection = new SqlConnection(@""))
            {
                connection.Open();

                var outputSerializer = new XmlSerializer(typeof(DataSchemaModel));
                var extractor = new SqlPublicationExtractor();
                outputSerializer.Serialize(new FileStream(@"Samples\Extracted Model.xml", FileMode.Create), extractor.Extract(connection));
            }
        }
    }
}
