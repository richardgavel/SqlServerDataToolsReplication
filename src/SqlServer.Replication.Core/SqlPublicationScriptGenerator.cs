using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Xml.Serialization;
using SqlServer.Replication.Core.Deployment;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core
{
    /// <summary>
    /// Generates one or more SQL scripts that, once applied to the target, will make the publications look like the source
    /// </summary>
    public class SqlPublicationScriptGenerator
    {
        public void Generate(string sourcePackage, string targetPackage, string outputFile)
        {
            Generate(Package.Open(sourcePackage), Package.Open(targetPackage), new FileInfo(outputFile));
        }

        public void Generate(Package sourcePackage, Package targetPackage, FileInfo outputFile)
        {
            var serializer = new XmlSerializer(typeof(DataSchemaModel));
            var uri = PackUriHelper.CreatePartUri(new Uri("/replication.xml", UriKind.Relative));

            var sourceModel = (DataSchemaModel)serializer.Deserialize(sourcePackage.GetPart(uri).GetStream());
            var targetModel = (DataSchemaModel)serializer.Deserialize(targetPackage.GetPart(uri).GetStream());

            var outputFileSql = new List<string>();

            foreach (var publicationToCreate in sourceModel.Model.Elements.Except(targetModel.Model.Elements, x => x.Name))
            {
                var createPublicationStep = new CreatePublicationStep(publicationToCreate);
                outputFileSql.AddRange(createPublicationStep.GenerateTSQL());
            }

            foreach (var publicationToAlter in sourceModel.Model.Elements.Intersect(targetModel.Model.Elements, x => x.Name))
            {
                var sqlPublicationComparer = new SqlPublicationComparer();
                var matchingPublicationInTarget = targetModel.Model.Elements.Single(x => x.Name == publicationToAlter.Name);

                var alterPublicationStep = new AlterPublicationStep(sqlPublicationComparer.Compare(publicationToAlter, matchingPublicationInTarget));
                outputFileSql.AddRange(alterPublicationStep.GenerateTSQL());
            }

            foreach (var publicationToDrop in targetModel.Model.Elements.Except(sourceModel.Model.Elements, x => x.Name))
            {
                var dropPublicationStep = new DropPublicationStep(publicationToDrop);
                outputFileSql.AddRange(dropPublicationStep.GenerateTSQL());
            }
        }
    }
}
