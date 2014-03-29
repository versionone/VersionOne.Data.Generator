using System;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Project
    {

        #region "PROPERTIES"
        public IList<RegressionTest> RegressionTests { get; set; }
        public IList<RegressionSuite> RegressionSuites { get; set; }
        public IList<RegressionPlan> RegressionPlans { get; set; }
        //public IList<RegressionTestSet> TestSets { get; set; }
        public IList<Environment> Environments { get; set; }
        public IterationSchedule Schedule { get; private set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<Member> Members { get; set; }
        public IList<Iteration> Iterations { get; set; }
        public IList<Theme> Themes { get; set; }
        public IList<Story> Stories { get; set; }
        public IList<Defect> Defects { get; set; }
        public IList<Epic> Epics { get; set; }
        public IList<Project> Releases { get; set; }
        public IList<Goal> Goals { get; set; }
        public IList<Request> Requests { get; set; }
        public IList<Issue> Issues { get; set; }
        public IList<BuildProject> BuildProjects { get; set; }
        public IList<BuildRun> BuildRuns { get; set; }
        public IList<ChangeSet> ChangeSets { get; set; }
        public TeamRoom TeamRoom { get; set; }
        public bool HasSchedule { get {return Schedule != null; } }
        #endregion

        #region "CONSTRUCTORS"
        public Project(string name, DateTime beginDate, DateTime endDate) : this(name, beginDate, endDate, null, null) {}
        public Project(string name, DateTime beginDate, DateTime endDate, int? iterationLength, int? iterationGap)
        {
            Releases = new List<Project>();
            Stories = new List<Story>();
            Defects = new List<Defect>();
            Epics = new List<Epic>();
            Themes = new List<Theme>();
            Iterations = new List<Iteration>();
            Members = new List<Member>();
            Environments = new List<Environment>();
            //TestSets = new List<RegressionTestSet>();
            RegressionPlans = new List<RegressionPlan>();
            RegressionSuites = new List<RegressionSuite>();
            RegressionTests = new List<RegressionTest>();
            Goals = new List<Goal>();
            Requests = new List<Request>();
            Issues = new List<Issue>();
            BuildProjects = new List<BuildProject>();
            BuildRuns = new List<BuildRun>();
            ChangeSets = new List<ChangeSet>();
            Name = name;
            BeginDate = beginDate;
            EndDate = endDate;
            if (iterationLength != null && iterationGap != null)
                Schedule = new IterationSchedule((int)iterationLength, (int)iterationGap);
        }
        #endregion

        #region "STATIC METHODS"

        public static IList<Project> GetProjects(int projectSet)
        {
            return new List<Project>
            {
                CreateProject(GetProjectName("Joel", projectSet)),
                CreateProject(GetProjectName("Adam", projectSet)),
                CreateProject(GetProjectName("Betsy", projectSet)),
                CreateProject(GetProjectName("Carl", projectSet)),
                CreateProject(GetProjectName("Diane", projectSet)),
                CreateProject(GetProjectName("Edward", projectSet)),
                CreateProject(GetProjectName("Fiona", projectSet)),
                CreateProject(GetProjectName("Guy", projectSet)),
                CreateProject(GetProjectName("Harvey", projectSet)),
                CreateProject(GetProjectName("Iris", projectSet)),
                CreateProject(GetProjectName("Justin", projectSet)),
                CreateProject(GetProjectName("Kelsey", projectSet)),
                CreateProject(GetProjectName("Lonnie", projectSet)),
                CreateProject(GetProjectName("Mary", projectSet)),
                CreateProject(GetProjectName("Nate", projectSet)),
                CreateProject(GetProjectName("Olivia", projectSet)),
                CreateProject(GetProjectName("Paul", projectSet)),
                CreateProject(GetProjectName("Quinn", projectSet)),
                CreateProject(GetProjectName("Rhonda", projectSet)),
                CreateProject(GetProjectName("Steve", projectSet)),
                CreateProject(GetProjectName("Ted", projectSet)),
                CreateProject(GetProjectName("Ursala", projectSet)),
                CreateProject(GetProjectName("Victor", projectSet)),
                CreateProject(GetProjectName("Wendy", projectSet)),
                CreateProject(GetProjectName("Xavier", projectSet)),
                CreateProject(GetProjectName("Yasmine", projectSet)),
                CreateProject(GetProjectName("Zach", projectSet)),
                CreateProject(GetProjectName("Ashleigh", projectSet)),
                CreateProject(GetProjectName("Lindsay", projectSet)),
                CreateProject(GetProjectName("Sydney", projectSet)),
                CreateProject(GetProjectName("Yngwie", projectSet)),
                CreateProject(GetProjectName("Angus", projectSet)),
                CreateProject(GetProjectName("Randy", projectSet)),
                CreateProject(GetProjectName("Jimi", projectSet)),
                CreateProject(GetProjectName("Eddie", projectSet)),
                CreateProject(GetProjectName("Eric", projectSet)),
                CreateProject(GetProjectName("Buddy", projectSet)),
                CreateProject(GetProjectName("Stevie", projectSet)),
                CreateProject(GetProjectName("Robert", projectSet)),
                CreateProject(GetProjectName("Billy", projectSet))
             };
        }

        private static string GetProjectName(string name, int projectSet)
        {
            if (projectSet == 0) return name;
            else return name += " " + projectSet.ToString();
        }

        public static Project CreateProject(string name)
        {
            Utils.Logger.Info(string.Format("\t-> Creating project {0} Bookstore...", name));

            DateTime beginDate;
            DateTime endDate;
            int iterationLength;
            int iterationGap;

            switch (Program._v1ClientTarget)
            {
                case "CapitalOne":
                {
                    beginDate = Utils.GetTodaysDateNoTime().AddDays(-14);
                    endDate = beginDate.AddDays(270);
                    iterationLength = 14;
                    iterationGap = 0;
                    break;
                }
                default:
                {
                    beginDate = Utils.GetTodaysDateNoTime();
                    endDate = Utils.GetTodaysDateNoTime().AddDays(270);
                    iterationLength = 7;
                    iterationGap = 0;
                    break;
                }
            }

            Project project = new Project(name + " Bookstore", beginDate, endDate, iterationLength, iterationGap);
            Member.AddMembers(name, project);
            Environment.AddProductEnvironments(project);
            RegressionPlan.AddRegressionPlans(project);
            RegressionSuite.AddRegressionSuites(project);
            RegressionTest.AddRegressionTests(project);
            Iteration.AddIterations(project);
            Project.AddRelease(name, project);
            Project.AddProjectBacklog(project);
            Request.AddProductRequests(project);
            Theme.AddThemes(project);
            return project;
        }

        //Add all of the assets here that you want included in the product backlog (i.e. not the release).
        public static void AddProjectBacklog(Project project)
        {
            // ToDo - the rest of these should be refactored to be methods in the corresponding classes 
            // (just like the release assets and also Request below).
            project.Stories.Add(new Story("Check Out", 2, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("Pay with Credit Card", 5, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("Delete Order", 1, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("Order Confirmation", 2, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("View Status of Order", 10, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("Calculate Shipping", 1, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("Validate Credit Card", 2, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Stories.Add(new Story("Rate Book", 2, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeStorefront));
            project.Stories.Add(new Story("Sign up for BookClub", 2, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeAcctMgmt));
            project.Stories.Add(new Story("Login", 3, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeAcctMgmt));
            project.Stories.Add(new Story("Setup Account", 3, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeAcctMgmt));
            project.Stories.Add(new Story("Update Account", 3, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeAcctMgmt));

            // These will be in the product backlog (not release) - therefore, assign a medium priority
            project.Defects.Add(new Defect("Check Out fails with 1-click", 1, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Defects.Add(new Defect("Login timeout when integrated security", 1, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeAcctMgmt));
            project.Defects.Add(new Defect("Intermittent credit card processing failure", 1, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeOrderProc));
            project.Defects.Add(new Defect("Signup hangs in offline mode", 1, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeAcctMgmt));
        }

        //Add all of the assets that you want included in the release (i.e. not the Product Backlog).
        public static void AddRelease(string name, Project project)
        {
            DateTime beginDate;
            DateTime endDate;

            switch (Program._v1ClientTarget)
            {
                case "CapitalOne":
                {
                    beginDate = Utils.GetTodaysDateNoTime().AddDays(-14);
                    endDate = beginDate.AddDays(90);
                    break;
                }
                default:
                {
                    beginDate = Utils.GetTodaysDateNoTime();
                    endDate = Utils.GetTodaysDateNoTime().AddDays(90);
                    break;
                }
            }

            Project release = new Project(name + "Release 1", beginDate, endDate);

            Epic.AddReleaseEpics(release);
            Story.AddReleaseStories(release);
            Story.AddIterationStories(release, project.Iterations[0]);
            Defect.AddReleaseDefects(release);
            Defect.AddIterationDefects(release, project.Iterations[0]);

            Environment.AddReleaseEnvironments(release);
            Goal.AddReleaseGoals(release);
            Issue.AddReleaseIssues(release);
            BuildProject.AddReleaseBuildProject(release);
            BuildRun.AddReleaseBuildRuns(release);
            ChangeSet.AddReleaseChangeSets(release);

            project.Releases.Add(release);
        }

        public static void SaveProjects(IEnumerable<Project> projects, string scopeId, IList<Member> owners, Asset schedule, bool isUltimate, bool isCatalyst, bool isEnterprisePlus, bool useTeamRoom)
        {
            IAssetType scopeType = Program.MetaModel.GetAssetType("Scope");
            IAttributeDefinition scopeName = scopeType.GetAttributeDefinition("Name");
            IAttributeDefinition scopeBeginDate = scopeType.GetAttributeDefinition("BeginDate");
            IAttributeDefinition scopeEndDate = scopeType.GetAttributeDefinition("EndDate");
            IAttributeDefinition scheduleDef = scopeType.GetAttributeDefinition("Schedule");

            Oid parentScope = Oid.FromToken(scopeId, Program.MetaModel);
            Asset useThisSchedule = schedule;

            foreach (Project project in projects)
            {
                Utils.Logger.Info(string.Format("\t-> Saving project {0}...", project.Name));
                Asset asset = Program.Services.New(scopeType, parentScope);
                asset.SetAttributeValue(scopeName, project.Name);
                asset.SetAttributeValue(scopeBeginDate, project.BeginDate);
                asset.SetAttributeValue(scopeEndDate, project.EndDate);

                //Catalyst only supports use of a single default schedule.
                if (isCatalyst == true)
                {
                    useThisSchedule = IterationSchedule.GetDefaultSchedule();
                }
                else if (project.HasSchedule)
                {
                    useThisSchedule = IterationSchedule.CreateSchedule(project.Name, project.Schedule);
                }

                asset.SetAttributeValue(scheduleDef, useThisSchedule.Oid);
                Program.Services.Save(asset);
                project.Id = asset.Oid.Momentless.Token;

                if (project.Members.Count > 0)
                    owners = project.Members;
                Member.SaveMembers(project);

                //Only Ultimate supports Environments.
                if (isUltimate)
                    Environment.SaveEnvironments(project);

                //Catalyst does not support Themes.
                if (isCatalyst == false)
                    Theme.SaveThemes(project);

                //Cataylst only supports a single default schedule.
                if (isCatalyst == true)
                    Iteration.SaveDefaultIterations(project);
                else
                    Iteration.SaveIterations(project);

                //Process the releases.
                SaveProjects(project.Releases, project.Id, owners, useThisSchedule, isUltimate, isCatalyst, isEnterprisePlus, useTeamRoom);

                //Catalyst does not support Epics.
                if (isCatalyst == false)
                    Epic.SaveEpics(project, owners);

                Story.SaveStories(project, owners, isCatalyst);

                //Catalyst does not support Requests.
                if (isCatalyst == false)
                {
                    Request.SaveRequests(project);
                    Request.GenerateStoryFromRequest(project);
                }

                Defect.SaveDefects(project, owners, isCatalyst);

                //Catalyst does not support Epics.
                if (isCatalyst == false)
                    Epic.AddStoriesToEpic(project);

                //Catalyst does not support Goals.
                if (isCatalyst == false)
                {
                    Goal.SaveGoals(project);
                    Goal.AddStoriesToGoal(project);
                }

                Issue.SaveIssues(project, owners);
                Issue.BlockDefectWithIssue(project);
                BuildProject.SaveBuildProjects(project);
                BuildRun.SaveBuildRuns(project);
                ChangeSet.SaveChangeSets(project);
                BuildRun.SaveItemAssociations(project);

                //Catalyst does not support TeamRooms.
                if (isCatalyst == false)
                {
                    if (project.HasSchedule == true && useTeamRoom == true)
                    {
                        TeamRoom.CreateTeamRoom(project, useThisSchedule);
                    }
                }

                if (isUltimate || isEnterprisePlus)
                {
                    RegressionPlan.SaveRegressionPlans(project);
                    RegressionSuite.SaveRegressionSuites(project, owners);
                    RegressionTest.SaveRegressionTests(project.RegressionTests, project);
                }
            }
        }

        // We decided we don't like the 'Future' backlog status and so we will inactivate it.
        public static void HideFutureStatus()
        {
            Oid token = Oid.FromToken(Utils.StatusFuture, Program.MetaModel);
            IOperation op = Program.MetaModel.GetOperation("StoryStatus.Inactivate");
            Program.Services.ExecuteOperation(op, token);
        }
        #endregion
    }
}
