using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class RegressionSuite
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public int RegressionPlanForSuite { get; set; }
        public IList<RegressionTest> RegressionTests { get; set; }
        public IList<RegressionTestSet> RegressionTestSets { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public RegressionSuite(string name, int regressionPlan)
        {
            RegressionTestSets = new List<RegressionTestSet>();
            RegressionTests = new List<RegressionTest>();
            Name = name;
            RegressionPlanForSuite = regressionPlan;
        }
        #endregion

        #region "STATIC METHODS"

        // This method creates Regression Suites AND the Regression Tests that are associated to them.
        // To create RTs just for the inventory add them via the RegressionTest class.
        public static void AddRegressionSuites(Project project)
        {
            RegressionSuite suite;

            suite = new RegressionSuite("Login/out", 0);
            suite.RegressionTests.Add(new RegressionTest("Login - Web security", "", "", "Login"));
            suite.RegressionTests.Add(new RegressionTest("Login - Integrated security", "", "", "Login"));
            suite.RegressionTests.Add(new RegressionTest("Logout check", "", "", "Logout"));
            suite.RegressionTests.Add(new RegressionTest("Session restored from previous", "", "", "User Login"));
            RegressionTestSet.AddReleaseTestSets(suite);
            project.RegressionSuites.Add(suite);

            suite = new RegressionSuite("Settings", 0);
            suite.RegressionTests.Add(new RegressionTest("Update User Settings", "", "", "User Account"));
            suite.RegressionTests.Add(new RegressionTest("Save User Settings", "", "", "User Account"));
            project.RegressionSuites.Add(suite);
            
            suite = new RegressionSuite("Bookstore Quicklist", 1);
            suite.RegressionTests.Add(new RegressionTest("Listings by Genre", "", "", "Quicklist"));
            suite.RegressionTests.Add(new RegressionTest("Recommended listings based on prior purchases", "", "", "Quicklist User"));
            project.RegressionSuites.Add(suite);
            
            suite = new RegressionSuite("Cart", 1);
            suite.RegressionTests.Add(new RegressionTest("Cart persisted across sessions", "", "", "Cart"));
            suite.RegressionTests.Add(new RegressionTest("Add/Remove Cart Item", "", "", "Account Cart"));
            project.RegressionSuites.Add(suite);

            suite = new RegressionSuite("Search", 1);
            suite.RegressionTests.Add(new RegressionTest("Search books by Genre", "", "", "Search"));
            suite.RegressionTests.Add(new RegressionTest("Search books by ISBN", "", "", "Search"));
            suite.RegressionTests.Add(new RegressionTest("Search books by Title", "", "", "Search"));
            project.RegressionSuites.Add(suite);
            
            suite = new RegressionSuite("Cancel Order", 2);
            suite.RegressionTests.Add(new RegressionTest("Order not Processed", "", "", "Orders"));
            suite.RegressionTests.Add(new RegressionTest("Order in Shipping", "", "", "Orders"));
            project.RegressionSuites.Add(suite);

            suite = new RegressionSuite("Checkout", 2);
            suite.RegressionTests.Add(new RegressionTest("1-Click Order", "", "", "Orders"));
            suite.RegressionTests.Add(new RegressionTest("Send as Gift Item", "", "", "Orders"));
            project.RegressionSuites.Add(suite);
            
            suite = new RegressionSuite("Special Orders", 2);
            suite.RegressionTests.Add(new RegressionTest("Apply Coupon Code", "", "", "Orders"));
            suite.RegressionTests.Add(new RegressionTest("Super Saver Shipping", "", "", "Orders"));                       
            project.RegressionSuites.Add(suite);
        }

        public static void SaveRegressionSuites(Project project, IList<Member> owners )
        {
            IAssetType suiteType = Program.MetaModel.GetAssetType("RegressionSuite");
            IAttributeDefinition suiteName = suiteType.GetAttributeDefinition("Name");
            IAttributeDefinition suiteRegressionPlan = suiteType.GetAttributeDefinition("RegressionPlan");
            IAttributeDefinition regressionTests = suiteType.GetAttributeDefinition("RegressionTests");

            foreach (RegressionSuite suite in project.RegressionSuites)
            {
                Asset asset = Program.Services.New(suiteType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(suiteName, suite.Name);

                RegressionTest.SaveRegressionTests(suite.RegressionTests, project);

                RegressionPlan plan = project.RegressionPlans[suite.RegressionPlanForSuite];
                asset.SetAttributeValue(suiteRegressionPlan, Oid.FromToken(plan.Id, Program.MetaModel));

                foreach (RegressionTest test in suite.RegressionTests)
                {
                    asset.AddAttributeValue(regressionTests, Oid.FromToken(test.Id, Program.MetaModel));
                }

                Program.Services.Save(asset);
                suite.Id = asset.Oid.Momentless.Token;
                RegressionTestSet.SaveTestSets(suite, project, owners);
            }
        }
        #endregion
    }
}
