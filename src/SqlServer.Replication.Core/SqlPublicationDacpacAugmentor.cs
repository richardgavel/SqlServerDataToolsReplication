using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Net.Mime;
using System.Xml.Serialization;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Augments a DACPAC by adding information about replications within the database
    /// </summary>
    public class SqlPublicationDacpacAugmentor
    {
        public void Augment(string dacpacPath, DataSchemaModel model)
        {
            var serializer = new XmlSerializer(typeof(DataSchemaModel));
            var modelStream = new MemoryStream();
            serializer.Serialize(modelStream, model);

            var package = Package.Open(dacpacPath);

            var uri = PackUriHelper.CreatePartUri(new Uri("/replication.xml", UriKind.Relative));
            var part = package.CreatePart(uri, MediaTypeNames.Text.Xml);

            Debug.Assert(part != null, "part != null");

            modelStream.Seek(0, SeekOrigin.Begin);
            modelStream.CopyTo(part.GetStream());

            package.Close();
        }
    }
}
