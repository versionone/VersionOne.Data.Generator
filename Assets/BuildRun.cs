using System;
using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class BuildRun
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Reference { get; set; }
        public string Source { get; set; }
        public string Status { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public BuildRun(string name, string reference, string source, string status, DateTime date)
        {
            Name = name;
            Reference = reference;
            Source = source;
            Status = status;
            Date = date;
        }
        #endregion

        #region "METHODS"
        public DateTime Date { get; set; }
        #endregion

        #region "STATIC METHODS"
        public static void AddReleaseBuildRuns(Project project)
        {
            project.BuildRuns.Add(new BuildRun( "Build Run - CCNet - 3.1.095", "1005", Utils.BuildRunSourceTrigger, Utils.BuildRunStatusPassed, DateTime.Now.AddDays( -2 )));
            project.BuildRuns.Add(new BuildRun( "Build Run - CCNet - 3.1.101", "1006", Utils.BuildRunSourceTrigger, Utils.BuildRunStatusFailed, DateTime.Now.AddDays( -1 )));
            project.BuildRuns.Add(new BuildRun( "Build Run - CCNet - 3.1.102", "1007", Utils.BuildRunSourceForced, Utils.BuildRunStatusPassed, DateTime.Now));
        }

        public static void SaveBuildRuns(Project project)
        {
            IAssetType buildRunType = Program.MetaModel.GetAssetType("BuildRun");
            IAttributeDefinition nameDef = buildRunType.GetAttributeDefinition("Name");
            IAttributeDefinition refDef = buildRunType.GetAttributeDefinition("Reference");
            IAttributeDefinition sourceDef = buildRunType.GetAttributeDefinition("Source");
            IAttributeDefinition statusDef = buildRunType.GetAttributeDefinition("Status");
            IAttributeDefinition dateDef = buildRunType.GetAttributeDefinition("Date");
            IAttributeDefinition buildProjectDef = buildRunType.GetAttributeDefinition("BuildProject");

            foreach (BuildRun buildRun in project.BuildRuns)
            {
                Asset asset = Program.Services.New(buildRunType, null);
                asset.SetAttributeValue(nameDef, buildRun.Name);
                asset.SetAttributeValue(refDef, buildRun.Reference);
                asset.SetAttributeValue(sourceDef, Oid.FromToken(buildRun.Source, Program.MetaModel));
                asset.SetAttributeValue(statusDef, Oid.FromToken(buildRun.Status, Program.MetaModel));
                asset.SetAttributeValue(dateDef, buildRun.Date);
                asset.SetAttributeValue(buildProjectDef, Oid.FromToken(project.BuildProjects[0].Id, Program.MetaModel));

                Program.Services.Save(asset);
                buildRun.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void SaveItemAssociations(Project project)
        {
            if (project.BuildRuns.Count > 0)
            {
                // There is no direct association between workitems and affected/included build runs.
                // The indirect association occurs by relationship to ChangeSets.

                string defectId = project.Defects.First(x => x.Name.Equals("Fix performance SLA on load")).Id;

                IAssetType defectType = Program.MetaModel.GetAssetType("Defect");
                Query query = new Query(Oid.FromToken(defectId, Program.MetaModel));

                IAttributeDefinition foundDefectsDef = defectType.GetAttributeDefinition("FoundInBuildRuns");
                IAttributeDefinition completedDef = defectType.GetAttributeDefinition("CompletedInBuildRuns");
                IAttributeDefinition changeSetsDef = defectType.GetAttributeDefinition("ChangeSets");

                QueryResult result = Program.Services.Retrieve(query);
                Asset asset = result.Assets[0];

                // Found in first build run and completed in last build run
                // Included in build runs 2 and 3 (via ChangeSets)
                asset.AddAttributeValue(foundDefectsDef, Oid.FromToken(project.BuildRuns[0].Id, Program.MetaModel));
                asset.AddAttributeValue(completedDef, Oid.FromToken(project.BuildRuns[2].Id, Program.MetaModel));
                asset.AddAttributeValue(changeSetsDef, Oid.FromToken(project.ChangeSets[1].Id, Program.MetaModel));
                asset.AddAttributeValue(changeSetsDef, Oid.FromToken(project.ChangeSets[2].Id, Program.MetaModel));
                Program.Services.Save(asset);

                string storyId = project.Stories.First(x => x.Name.Equals("List of Books by Author")).Id;

                IAssetType storyType = Program.MetaModel.GetAssetType("Story");
                query = new Query(Oid.FromToken(storyId, Program.MetaModel));

                IAttributeDefinition completedStoryDef = storyType.GetAttributeDefinition("CompletedInBuildRuns");
                IAttributeDefinition changeSetsStoryDef = storyType.GetAttributeDefinition("ChangeSets");

                result = Program.Services.Retrieve(query);
                asset = result.Assets[0];

                // Completed in last build run
                // Included in build runs 2 and 3 (via ChangeSets)
                asset.AddAttributeValue(completedStoryDef, Oid.FromToken(project.BuildRuns[2].Id, Program.MetaModel));
                asset.AddAttributeValue(changeSetsStoryDef, Oid.FromToken(project.ChangeSets[1].Id, Program.MetaModel));
                asset.AddAttributeValue(changeSetsStoryDef, Oid.FromToken(project.ChangeSets[2].Id, Program.MetaModel));
                Program.Services.Save(asset);

                storyId = project.Stories.First(x => x.Name.Equals("View Book Details")).Id;
                query = new Query(Oid.FromToken(storyId, Program.MetaModel));

                result = Program.Services.Retrieve(query);
                asset = result.Assets[0];

                // Included in all 3 build runs (via ChangeSets)
                // Not completed yet
                asset.AddAttributeValue(changeSetsStoryDef, Oid.FromToken(project.ChangeSets[0].Id, Program.MetaModel));                
                asset.AddAttributeValue(changeSetsStoryDef, Oid.FromToken(project.ChangeSets[1].Id, Program.MetaModel));
                asset.AddAttributeValue(changeSetsStoryDef, Oid.FromToken(project.ChangeSets[2].Id, Program.MetaModel));
                Program.Services.Save( asset );
            }
        }
        #endregion
    }
}
