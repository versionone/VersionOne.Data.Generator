using System;
using System.Collections.Generic;
using VersionOne.Data.Generator.Interfaces;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Story : IWorkitem
    {
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

        #region "CONSTRUCTORS"
        public Story(string name, int estimate, string priorityId, string statusId, Theme theme)
            : this(name, estimate, priorityId, statusId, null, theme, null) { }
        public Story(string name, int estimate, string priorityId, string statusId, Theme theme, Team team)
            : this(name, estimate, priorityId, statusId, null, theme, team) { }
        public Story(string name, int estimate, string priorityId, string statusId, Iteration iteration, Theme theme, Team team)
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

        #region "STATIC METHODS"

        public static void AddReleaseStories(Project project)
        {
            project.Stories.Add(new Story("Search by Keyword", 2, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeStorefront, Team.TeamAlpha)); //0
            project.Stories.Add(new Story("View Account Details", 2, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeStorefront, Team.TeamBeta)); //1
            project.Stories.Add(new Story("Add Book to Shopping Cart", 2, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeOrderProc, Team.TeamBeta)); //2
            project.Stories.Add(new Story("Remove Book from Shopping Cart", 1, Utils.PriorityMedium, Utils.StatusNone, Theme.ThemeOrderProc, Team.TeamAlpha)); //3
            project.Stories.Add(new Story("Save Order", 3, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeOrderProc, Team.TeamBeta)); //4
            project.Stories.Add(new Story("List of Books by ISBN", 3, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeStorefront, Team.TeamAlpha)); //5
            project.Stories.Add(new Story("Search by Title", 10, Utils.PriorityHigh, Utils.StatusNone, Theme.ThemeStorefront, Team.TeamBeta)); //6
            project.Stories.Add(new Story("Search by Genre", 2, Utils.PriorityLow, Utils.StatusNone, Theme.ThemeStorefront, Team.TeamAlpha)); //7
        }

        public static void AddIterationStories(Project project, Iteration iteration)
        {
            project.Stories.Add(CreateIterationStory("List of Books by Author", 1, Utils.PriorityHigh, Utils.StatusNone, iteration, Theme.ThemeStorefront, Team.TeamBeta)); 
            project.Stories.Add(CreateIterationStory("List of Books by Title", 3, Utils.PriorityMedium, Utils.StatusNone, iteration, Theme.ThemeStorefront, Team.TeamAlpha)); 
            project.Stories.Add(CreateIterationStory("List of Books by Genre", 5, Utils.PriorityHigh, Utils.StatusNone, iteration, Theme.ThemeStorefront, Team.TeamBeta)); 
            project.Stories.Add(CreateIterationStory("View Book Details", 2, Utils.PriorityMedium, Utils.StatusNone, iteration, Theme.ThemeStorefront, Team.TeamBeta)); 
            project.Stories.Add(CreateIterationStory("Search by Author", 5, Utils.PriorityMedium, Utils.StatusDone, iteration, Theme.ThemeStorefront, Team.TeamAlpha)); 
        }

        private static Story CreateIterationStory(string name, int estimate, string priority, string status, Iteration iteration, Theme theme, Team team)
        {
            Story story = new Story(name, estimate, priority, status, iteration, theme, team);

            bool done = (status == Utils.StatusDone | status == Utils.StatusAccepted);
            Random r = new Random();
            int i;

            i = r.Next(1, 3);
            story.Tasks.Add(new Task("Create Database Tables", i, (done ? 0 : i)));
            i = r.Next(1, 16); 
            story.Tasks.Add(new Task("Design UI", i, (done ? 0 : i)));

            if (estimate > 1)
            {
                i = r.Next(1, 8);
                story.Tasks.Add(new Task("Code Business Objects", i, (done ? 0 : i)));
            }

            if (estimate > 2)
            {
                i = r.Next(1, 4);
                story.Tasks.Add(new Task("Write Javascript", i, (done ? 0 : r.Next(i))));
                i = r.Next(1, 4);
                story.Tasks.Add(new Task("Code DAO", i, (done ? 0 : i))); 
            }

            if (estimate > 3)
            {
                i = r.Next(1, 16);
                story.Tasks.Add(new Task("Write Integration Interface", i, (done ? 0 : i)));
            }

            string testName = name.Replace(" ", "");
            i = r.Next(1, 8);
            story.Tests.Add(new Test("Fitnesse Test " + testName, i, (done ? 0 : i))); 

            return story;
        }

        public static void SaveStories(Project project, IList<Member> owners, bool isCatalyst)
        {
            IAssetType storyType = Program.MetaModel.GetAssetType("Story");
            IAttributeDefinition storyName = storyType.GetAttributeDefinition("Name");
            IAttributeDefinition storyEstimate = storyType.GetAttributeDefinition("Estimate");
            IAttributeDefinition storyPriority = storyType.GetAttributeDefinition("Priority");
            IAttributeDefinition storyStatus = storyType.GetAttributeDefinition("Status");
            IAttributeDefinition storyTeam = storyType.GetAttributeDefinition("Team");
            IAttributeDefinition storyOwners = storyType.GetAttributeDefinition("Owners");
            IAttributeDefinition storyIteration = storyType.GetAttributeDefinition("Timebox");
            IAttributeDefinition storyTheme = storyType.GetAttributeDefinition("Parent");

            IList<Story> stories = project.Stories;

            foreach (Story story in stories)
            {
                Asset asset = Program.Services.New(storyType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(storyName, story.Name);
                asset.SetAttributeValue(storyEstimate, story.Estimate);
                asset.SetAttributeValue(storyPriority, Oid.FromToken(story.PriorityId, Program.MetaModel));
                if (!string.IsNullOrEmpty(story.StatusId))
                    asset.SetAttributeValue(storyStatus, Oid.FromToken(story.StatusId, Program.MetaModel));

                //Catalyst does not support Teams.
                if (isCatalyst == false)
                {
                    if (story.Team != null)
                        asset.SetAttributeValue(storyTeam, Oid.FromToken(story.Team.Id, Program.MetaModel));
                }

                if (story.Iteration != null)
                {
                    asset.SetAttributeValue(storyIteration, Oid.FromToken(story.Iteration.Id, Program.MetaModel));

                    // Only add owner if story is in iteration
                    asset.AddAttributeValue(storyOwners, Oid.FromToken(owners[0].Id, Program.MetaModel));
                }

                //Catalyst does not support Themes.
                if (isCatalyst == false)
                {
                    if (story.Theme != null)
                        asset.SetAttributeValue(storyTheme, Oid.FromToken(story.Theme.Id, Program.MetaModel));
                }

                Program.Services.Save(asset);
                story.Id = asset.Oid.Momentless.Token;

                Task.SaveTasks(story, owners);
                Test.SaveTests(story, owners);
            }
        }

        public static void UpdateStory(string storyId, string status)
        {
            Query query = new Query(Oid.FromToken(storyId, Program.MetaModel));
            IAssetType storyType = Program.MetaModel.GetAssetType("Story");
            IAttributeDefinition statusAttribute = storyType.GetAttributeDefinition("Status");
            QueryResult result = Program.Services.Retrieve(query);
            Asset story = result.Assets[0];
            story.SetAttributeValue(statusAttribute, Oid.FromToken(status, Program.MetaModel));
            Program.Services.Save(story);
        }

        public static void CloseStory(string storyId)
        {
            IOperation closeOperation = Program.MetaModel.GetOperation("Story.Inactivate");
            Program.Services.ExecuteOperation(closeOperation, Oid.FromToken(storyId, Program.MetaModel));
        }
        #endregion
    }
}
