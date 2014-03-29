using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class BuildProject
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public BuildProject(string name, string reference)
        {
            Name = name;
            Reference = reference;
        }
        #endregion

        #region "METHODS"
        public static void AddReleaseBuildProject(Project project)
        {
            project.BuildProjects.Add(new BuildProject(project.Name + " Builds", "Integration with CruiseControl.NET"));
        }

        public static void SaveBuildProjects(Project project)
        {
            IAssetType buildProjectType = Program.MetaModel.GetAssetType("BuildProject");
            IAttributeDefinition nameDef = buildProjectType.GetAttributeDefinition("Name");
            IAttributeDefinition refDef = buildProjectType.GetAttributeDefinition("Reference");
            IAttributeDefinition scopesDef = buildProjectType.GetAttributeDefinition("Scopes");

            foreach (BuildProject buildProject in project.BuildProjects)
            {
                Asset asset = Program.Services.New(buildProjectType, null);
                asset.SetAttributeValue(nameDef, buildProject.Name);
                asset.SetAttributeValue(refDef, buildProject.Reference);
                asset.AddAttributeValue(scopesDef, Oid.FromToken( project.Id, Program.MetaModel));

                Program.Services.Save(asset);
                buildProject.Id = asset.Oid.Momentless.Token;
            }
        }
        #endregion
    }
}
