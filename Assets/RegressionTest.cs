using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class RegressionTest
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Setup { get; set; }
        public string Steps { get; set; }
        public string Tags { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public RegressionTest(string name, string setup, string steps, string tags)
        {
            Name = name;
            Setup = setup;
            Steps = steps;
            Tags = tags;
        }
        #endregion

        #region "STATIC METHODS"

        // These tests exist in the inventory but are not assigned to any suite.
        public static void AddRegressionTests(Project project)
        {
            project.RegressionTests.Add(new RegressionTest("Valid user logs in", 
                                                           "Create user account user password.  Close Any Sessions, clean browser cache", 
                                                           "1. Open Browser\n 2.Go To homepage\n 3.Login with user password", 
                                                           "User Account Login"));
            project.RegressionTests.Add(new RegressionTest("Valid user logs out", 
                                                           "Close Any Sessions, clean browser cache", 
                                                           "1. Open Browser\n 2.Go To homepage\n 3.Login with user password\n 4.Click logout", 
                                                           "User Account Logout"));
            project.RegressionTests.Add(new RegressionTest("Unknown user tries to login", 
                                                           "Close Any Sessions, clean browser cache", 
                                                           "1. Open Browser\n 2.Go To homepage\n 3.Login with baduser password", 
                                                           "User Login LDAP"));
            project.RegressionTests.Add(new RegressionTest("Valid user logs in with invalid password", 
                                                           "Close Any Sessions, clean browser cache", 
                                                           "1. Open Browser\n 2.Go To homepage\n 3.Login with user badpassword", 
                                                           "User Login Credentials"));
        }

        public static void SaveRegressionTests(IEnumerable<RegressionTest> regressionTests, Project project)
        {
            IAssetType regressionTestType = Program.MetaModel.GetAssetType("RegressionTest");
            IAttributeDefinition regressionTestName = regressionTestType.GetAttributeDefinition("Name");
            IAttributeDefinition regressionTestSetup = regressionTestType.GetAttributeDefinition("Setup");
            IAttributeDefinition regressionTestSteps = regressionTestType.GetAttributeDefinition("Steps");
            IAttributeDefinition regressionTestTags = regressionTestType.GetAttributeDefinition("Tags");

            foreach (RegressionTest regressionTest in regressionTests)
            {
                Asset asset = Program.Services.New(regressionTestType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(regressionTestName, regressionTest.Name);
                asset.SetAttributeValue(regressionTestSetup, regressionTest.Setup);
                asset.SetAttributeValue(regressionTestSteps, regressionTest.Steps);
                asset.SetAttributeValue(regressionTestTags, regressionTest.Tags);
                Program.Services.Save(asset);
                regressionTest.Id = asset.Oid.Momentless.Token;
            }
        }
        #endregion
    }
}
