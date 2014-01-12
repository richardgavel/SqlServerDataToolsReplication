using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using SqlServer.Replication.Core;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.UnitTests
{
    [TestFixture]
    public class SqlPublicationComparerTests
    {
        [Test]
        public void Compare()
        {
            var comparer = new SqlPublicationComparer();

            var serializer = new XmlSerializer(typeof (Element));
            var source = (Element)serializer.Deserialize(new FileStream(@"Samples\Compiled Model.xml", FileMode.Open));
            var target = (Element)serializer.Deserialize(new FileStream(@"Samples\Extracted Model.xml", FileMode.Open));

            comparer.Compare(source, target);
        }
    }
}
