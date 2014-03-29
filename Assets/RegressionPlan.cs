using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class RegressionPlan
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public RegressionPlan(string name)
        {
            Name = name;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddRegressionPlans(Project project)
        {            
            project.RegressionPlans.Add(new RegressionPlan("Account Management"));
            project.RegressionPlans.Add(new RegressionPlan("Core Functionality"));
            project.RegressionPlans.Add(new RegressionPlan("Orders/Sales"));
        }

        public static void SaveRegressionPlans(Project project)
        {
            IAssetType regressionPlanType = Program.MetaModel.GetAssetType("RegressionPlan");
            IAttributeDefinition regressionPlanName = regressionPlanType.GetAttributeDefinition("Name");

            foreach (RegressionPlan plan in project.RegressionPlans)
            {
                Asset asset = Program.Services.New(regressionPlanType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(regressionPlanName, plan.Name);

                Program.Services.Save(asset);
                plan.Id = asset.Oid.Momentless.Token;
            }
        }
        #endregion
    }
}
