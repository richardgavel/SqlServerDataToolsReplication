using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.SqlServer.Dac.Model;
using NUnit.Framework;
using SqlServer.Replication.Core;
using SqlServer.Replication.Model.Compiled;
using Source = SqlServer.Replication.Model.Source;

namespace SqlServer.Replication.UnitTests
{
    [TestFixture]
    public class SqlPublicationCompilerTests
    {
        [Test]
        public void Execute()
        {
            var compiler = new SqlPublicationCompiler();

            var sourceSerializer = new XmlSerializer(typeof (Source.Publication));
            var source = (Source.Publication) sourceSerializer.Deserialize(new FileStream(@"Samples\Source File.xml", FileMode.Open));

            var model = new TSqlModel(@"");

            var output = compiler.Compile(new List<Source.Publication> { source }, model);

            var outputSerializer = new XmlSerializer(typeof(DataSchemaModel));
            outputSerializer.Serialize(new FileStream(@"Samples\Compiled Model.xml", FileMode.Create), output);
        }
    }
}
