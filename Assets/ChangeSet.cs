using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class ChangeSet
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reference { get; set; }
        public int BuildRun { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public ChangeSet(string name, string description, string reference, int buildRun)
        {
            Name = name;
            Description = description;
            Reference = reference;
            BuildRun = buildRun;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddReleaseChangeSets(Project project)
        {
            project.ChangeSets.Add(new ChangeSet("CS41754", "Baseline commit", "rev:41754", 0));
            project.ChangeSets.Add(new ChangeSet("CS41796", "Refactor - move methods to corresponding classes", "rev:41796", 1));
            project.ChangeSets.Add(new ChangeSet("CS41816", "JIRA Project Mapping, Sub-project Mapping, Enhanced Error Handling", "rev41816", 2));
        }

        public static void SaveChangeSets(Project project)
        {
            if (project.ChangeSets.Count > 0)
            {
                IAssetType changeSetType = Program.MetaModel.GetAssetType("ChangeSet");
                IAttributeDefinition nameDef = changeSetType.GetAttributeDefinition("Name");
                IAttributeDefinition descDef = changeSetType.GetAttributeDefinition("Description");
                IAttributeDefinition refDef = changeSetType.GetAttributeDefinition("Reference");
                IAttributeDefinition buildRunDef = changeSetType.GetAttributeDefinition("BuildRuns");

                foreach (ChangeSet changeSet in project.ChangeSets)
                {
                    Asset asset = Program.Services.New(changeSetType, null);
                    asset.SetAttributeValue(nameDef, changeSet.Name);
                    asset.SetAttributeValue(descDef, changeSet.Description);
                    asset.SetAttributeValue(refDef, changeSet.Reference);
                    asset.AddAttributeValue(buildRunDef, Oid.FromToken(project.BuildRuns[changeSet.BuildRun].Id, Program.MetaModel));

                    Program.Services.Save(asset);
                    changeSet.Id = asset.Oid.Momentless.Token;
                }
            }
        }
        #endregion
    }
}
