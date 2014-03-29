using VersionOne.SDK.APIClient;
using System.Linq;

namespace VersionOne.Data.Generator
{
    public class Goal
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PriorityId { get; set; }
        public string CategoryId { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Goal(string name, string description, string priorityId, string categoryId)
        {
            Name = name;
            Description = description;
            PriorityId = priorityId;
            CategoryId = categoryId;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddReleaseGoals(Project project)
        {
            project.Goals.Add(new Goal("Improve Performance by 5%", "", Utils.GoalPriorityLow, Utils.GoalCategoryCompetitive));
            project.Goals.Add(new Goal("Increase Cust Satisfaction by 10%", "", Utils.GoalPriorityHigh, Utils.GoalCategoryStrategic));
            project.Goals.Add(new Goal("Increase Prospect Conversion by 5%", "", Utils.GoalPriorityLow, Utils.GoalCategoryFinancial));
            project.Goals.Add(new Goal("Reduce Customer Support Calls by 20%", "", Utils.GoalPriorityMedium, Utils.GoalCategoryFinancial));
        }

        public static void SaveGoals(Project project)
        {
            IAssetType goalType = Program.MetaModel.GetAssetType("Goal");
            IAttributeDefinition nameDef = goalType.GetAttributeDefinition("Name");
            IAttributeDefinition descDef = goalType.GetAttributeDefinition("Description");
            IAttributeDefinition priorityDef = goalType.GetAttributeDefinition("Priority");
            IAttributeDefinition categoryDef = goalType.GetAttributeDefinition("Category");
            IAttributeDefinition targetedByDef = goalType.GetAttributeDefinition("TargetedBy");

            foreach (Goal goal in project.Goals)
            {
                Asset asset = Program.Services.New(goalType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(nameDef, goal.Name);
                asset.SetAttributeValue(descDef, goal.Description);
                asset.SetAttributeValue(priorityDef, Oid.FromToken(goal.PriorityId, Program.MetaModel));
                asset.SetAttributeValue(categoryDef, Oid.FromToken(goal.CategoryId, Program.MetaModel));
                asset.AddAttributeValue(targetedByDef, Oid.FromToken(project.Id, Program.MetaModel));

                Program.Services.Save(asset);
                goal.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void AddStoriesToGoal(Project project)
        {
            var matchingGoal = project.Goals.FirstOrDefault(x => x.Name.Equals("Increase Cust Satisfaction by 10%"));

            if (matchingGoal != null)
            {
                string goalId = matchingGoal.Id;

                Query query = new Query(Oid.FromToken(goalId, Program.MetaModel));
                IAttributeDefinition workitemsDef = Program.MetaModel.GetAttributeDefinition("Goal.Workitems");
                QueryResult result = Program.Services.Retrieve(query);
                Asset goal = result.Assets[0];

                string storyId1 = project.Stories.First(x => x.Name.Equals("List of Books by Author")).Id;
                string storyId2 = project.Stories.First(x => x.Name.Equals("List of Books by Genre")).Id;
                goal.AddAttributeValue(workitemsDef, Oid.FromToken(storyId1, Program.MetaModel));
                goal.AddAttributeValue(workitemsDef, Oid.FromToken(storyId2, Program.MetaModel));
                Program.Services.Save(goal);
            }
        }
        #endregion
    }
}
