using System;
using System.Collections.Generic;
using VersionOne.Data.Generator.Interfaces;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Defect : IWorkitem
    {
        #region "CONSTRUCTORS"
        public Defect( string name, int estimate, string priorityId, string statusId, Theme theme )
            : this( name, estimate, priorityId, statusId, null, theme, null ) { }
        public Defect( string name, int estimate, string priorityId, string statusId, Theme theme, Team team )
            : this( name, estimate, priorityId, statusId, null, theme, team ) { }
        public Defect( string name, int estimate, string priorityId, string statusId, Iteration iteration, Theme theme, Team team )
        {
            Tasks = new List<Task>();
            Tests = new List<Test>();
            Name = name;
            Estimate = estimate;
            PriorityId = priorityId;
            StatusId = statusId;
            Iteration = iteration;
            Team = team;
            Theme = theme;
        }
        #endregion

        #region "PROPERTIES"
        public IList<Test> Tests { get; set; }
        public IList<Task> Tasks { get; set; }
        public Iteration Iteration { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Estimate { get; set; }
        public string PriorityId { get; set; }
        public string StatusId { get; set; }
        public string OwnerId { get; set; }
        public Team Team { get; set; }
        public Theme Theme { get; set; }
        #endregion

        #region "STATIC METHODS"

        public static void AddReleaseDefects(Project project)
        {
            project.Defects.Add(new Defect("Fix book details when Genre 'mystery'", 2, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeStorefront, Team.TeamAlpha)); //0
            project.Defects.Add(new Defect("Incorrect response label when saving order", 1, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeOrderProc, Team.TeamBeta)); //1
            project.Defects.Add(new Defect("Title display incorrect on IE", 1, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeOrderProc, Team.TeamBeta)); //2
        }

        public static void AddIterationDefects(Project project, Iteration iteration)
        {
            project.Defects.Add(CreateIterationDefect("Fix performance SLA on load", 1, Utils.PriorityHigh, Utils.StatusInProgress, iteration, Theme.ThemeStorefront, Team.TeamBeta));
            project.Defects.Add(CreateIterationDefect("Correct warning message label", 3, Utils.PriorityMedium, Utils.StatusNone, iteration, Theme.ThemeStorefront, Team.TeamAlpha));
        }

        private static Defect CreateIterationDefect(string name, int estimate, string priority, string status, Iteration iteration, Theme theme, Team team )
        {
            bool done = (status == Utils.StatusDone | status == Utils.StatusAccepted);
            Random r = new Random();
            int i;

            Defect defect = new Defect(name, estimate, priority, status, iteration, theme, team);
            i = r.Next(1, 2);
            defect.Tasks.Add(new Task( "Verify fix", i, (done ? 0 : i)));
            i = r.Next(1, 16);
            defect.Tasks.Add(new Task( "Ensure standards", i, (done ? 0 : i)));
            string testName = name.Replace(" ", "");
            i = r.Next(1, 8);
            defect.Tests.Add(new Test("Fitnesse Test " + testName, i, (done ? 0 : i)));

            return defect;
        }

        public static void SaveDefects(Project project, IList<Member> owners, bool isCatalyst)
        {
            IAssetType defectType = Program.MetaModel.GetAssetType("Defect");
            IAttributeDefinition defectName = defectType.GetAttributeDefinition("Name");
            IAttributeDefinition defectEstimate = defectType.GetAttributeDefinition("Estimate");
            IAttributeDefinition defectPriority = defectType.GetAttributeDefinition("Priority");
            IAttributeDefinition defectStatus = defectType.GetAttributeDefinition("Status");
            IAttributeDefinition defectTeam = defectType.GetAttributeDefinition("Team");
            IAttributeDefinition defectOwners = defectType.GetAttributeDefinition("Owners");
            IAttributeDefinition defectIteration = defectType.GetAttributeDefinition("Timebox");
            IAttributeDefinition defectTheme = defectType.GetAttributeDefinition("Parent");

            IList<Defect> defects = project.Defects;

            foreach (Defect defect in defects)
            {
                Asset asset = Program.Services.New(defectType, Oid.FromToken( project.Id, Program.MetaModel));
                asset.SetAttributeValue(defectName, defect.Name);
                asset.SetAttributeValue(defectEstimate, defect.Estimate);
                asset.SetAttributeValue(defectPriority, Oid.FromToken(defect.PriorityId, Program.MetaModel));
                if ( !string.IsNullOrEmpty(defect.StatusId))
                    asset.SetAttributeValue(defectStatus, Oid.FromToken(defect.StatusId, Program.MetaModel));
                
                //Catalyst does not support Teams.
                if (isCatalyst == false)
                {
                    if (defect.Team != null)
                        asset.SetAttributeValue(defectTeam, Oid.FromToken(defect.Team.Id, Program.MetaModel));
                }
                
                if (defect.Iteration != null)
                {
                    asset.SetAttributeValue(defectIteration, Oid.FromToken(defect.Iteration.Id, Program.MetaModel));

                    // Only add owner if story is in iteration
                    asset.AddAttributeValue(defectOwners, Oid.FromToken(owners[0].Id, Program.MetaModel));
                }

                //Catalyst does not support Themes.
                if (isCatalyst == false)
                {
                    if (defect.Theme != null)
                        asset.SetAttributeValue(defectTheme, Oid.FromToken(defect.Theme.Id, Program.MetaModel));
                }
                
                Program.Services.Save(asset);
                defect.Id = asset.Oid.Momentless.Token;

                Task.SaveTasks(defect, owners);
                Test.SaveTests(defect, owners);
            }
        }
        #endregion
    }
}
