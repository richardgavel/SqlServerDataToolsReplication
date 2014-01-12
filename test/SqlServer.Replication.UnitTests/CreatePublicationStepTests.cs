using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using SqlServer.Replication.Core;
using SqlServer.Replication.Core.Deployment;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.UnitTests
{
    [TestFixture]
    public class CreatePublicationStepTests
    {
        [Test]
        public void RecreateExistingPublication()
        {
            using (var connection = new SqlConnection(@""))
            {
                connection.Open();

                var extractor = new SqlPublicationExtractor();
                var model = extractor.Extract(connection);

                var step = new CreatePublicationStep(model.Model.Elements[0]);
                var sql = step.GenerateTSQL();

                foreach (var line in sql)
                    Console.WriteLine(line);
            }
        }

        [Test]
        public void Execute()
        {
            var outputSerializer = new XmlSerializer(typeof(DataSchemaModel));
            var model = (DataSchemaModel)outputSerializer.Deserialize(new FileStream(@"Samples\Extracted Model.xml", FileMode.Open));

            var step = new CreatePublicationStep(model.Model.Elements[0]);
            var sql = step.GenerateTSQL();

            foreach (var line in sql)
                Console.WriteLine(line);
        }
    }
}
