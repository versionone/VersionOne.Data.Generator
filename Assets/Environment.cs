using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Environment
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Environment(string name)
        {
            Name = name;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddProductEnvironments(Project project)
        {
            project.Environments.Add(new Environment("Browser"));
            project.Environments.Add(new Environment("Phone"));
        }

        public static void AddReleaseEnvironments(Project project)
        {
            project.Environments.Add(new Environment("IE"));
            project.Environments.Add(new Environment("FireFox"));
        }

        public static void SaveEnvironments(Project project)
        {
            IAssetType environmentType = Program.MetaModel.GetAssetType("Environment");
            IAttributeDefinition environmentName = environmentType.GetAttributeDefinition("Name");
            IAttributeDefinition environmentScope = environmentType.GetAttributeDefinition("Scope");
            IList<Environment> envrionments = project.Environments;

            foreach (Environment environment in envrionments)
            {
                Asset asset = Program.Services.New(environmentType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(environmentName, environment.Name);
                asset.SetAttributeValue(environmentScope, Oid.FromToken(project.Id, Program.MetaModel));
                Program.Services.Save(asset);
                environment.Id = asset.Oid.Momentless.Token;
            }
        }
        #endregion   
    }
}
