using System.Collections.Generic;
using Microsoft.SqlServer.Dac.Deployment;
using SqlServer.Replication.Model.Compiled;

namespace SqlServer.Replication.Core.Deployment
{
    /// <summary>
    /// Deployment step that generates the SQL necessary to drop a publication based upon a compiled model
    /// </summary>
    public class DropPublicationStep : DeploymentStep
    {
        public DropPublicationStep(Element targetElement)
        {
            TargetElement = targetElement;
        }

        public Element TargetElement { get; private set; }

        public override IList<string> GenerateTSQL()
        {
            var result = new List<string>();

            result.Add(string.Format("sp_droppublication @publication = '{0}'", TargetElement.Name));
            result.Add(string.Empty);

            return result;
        }
    }
}
