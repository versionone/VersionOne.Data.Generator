using System.Collections.Generic;
using VersionOne.SDK.APIClient;
using System.Linq;

namespace VersionOne.Data.Generator
{
    public class Epic
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Epic(string name)
        {
            Name = name;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddReleaseEpics(Project project)
        {
            project.Epics.Add(new Epic("Search for Books")); //0
        }

        public static void SaveEpics(Project project, IList<Member> owners)
        {
            IAssetType epicType = Program.MetaModel.GetAssetType("Epic");
            IAttributeDefinition nameDef = epicType.GetAttributeDefinition("Name");
            IAttributeDefinition ownersDef = epicType.GetAttributeDefinition("Owners");

            foreach (Epic epic in project.Epics)
            {
                Asset asset = Program.Services.New(epicType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(nameDef, epic.Name);
                asset.AddAttributeValue(ownersDef, Oid.FromToken(owners[0].Id, Program.MetaModel));
                Program.Services.Save(asset);
                epic.Id = asset.Oid.Momentless.Token;
            }
        }

        // 8/7/2012 AJB Commented out to replace with new version of method based on API changes for Epics.
        //public static void SaveEpics(Project project, IList<Member> owners)
        //{
        //    IAssetType storyType = Program.MetaModel.GetAssetType("Story");
        //    IAttributeDefinition nameDef = storyType.GetAttributeDefinition("Name");
        //    IAttributeDefinition ownersDef = storyType.GetAttributeDefinition("Owners");
        //    Utils.Logger.Info("ABOUT TO SAVE EPICS FOR: " + project.Name);

        //    foreach (Epic epic in project.Epics)
        //    {
        //        Asset asset = Program.Services.New(storyType, Oid.FromToken(project.Id, Program.MetaModel));
        //        asset.SetAttributeValue(nameDef, epic.Name);
        //        asset.AddAttributeValue(ownersDef, Oid.FromToken(owners[0].Id, Program.MetaModel));

        //        Program.Services.Save(asset);
        //        epic.Id = asset.Oid.Momentless.Token;

        //        IOperation storyToEpic = storyType.GetOperation("StoryToEpic");
        //        Program.Services.ExecuteOperation(storyToEpic, asset.Oid);
        //    }
        //}

        public static void AddStoriesToEpic(Project project)
        {
            var stories = project.Stories.Where(x => x.Name.StartsWith("Search by"));

            foreach (Story item in stories)
            {
                Query query = new Query(Oid.FromToken(item.Id, Program.MetaModel));
                IAssetType storyType = Program.MetaModel.GetAssetType("Story");
                IAttributeDefinition superDef = storyType.GetAttributeDefinition("Super");
                QueryResult result = Program.Services.Retrieve(query);
                Asset story = result.Assets[0];
                story.SetAttributeValue(superDef, project.Epics[0].Id);
                Program.Services.Save(story);
            }
        }
        #endregion
    }
}
