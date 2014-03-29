using System.Collections.Generic;
using VersionOne.Data.Generator.Interfaces;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Test
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string StatusId { get; set; }
        public int Estimate { get; set; }
        public string OwnerId { get; set; }
        public int Todo { get; set; }
        public RegressionTest GeneratedRegressionTests { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Test(string name, int estimate, int todo)
        {
            Name = name;
            Estimate = estimate;
            Todo = todo;
        }
        #endregion

        #region "STATIC METHODS"

        public static void SaveTests(IWorkitem item, IList<Member> owners)
        {
            IAssetType testType = Program.MetaModel.GetAssetType("Test");
            IAttributeDefinition testName = testType.GetAttributeDefinition("Name");
            IAttributeDefinition testEstimate = testType.GetAttributeDefinition("DetailEstimate");
            IAttributeDefinition testOwners = testType.GetAttributeDefinition("Owners");
            IAttributeDefinition testTodo = testType.GetAttributeDefinition("ToDo");

            IList<Test> tests = item.Tests;
            int ownerIndex = 1;

            foreach (Test test in tests)
            {
                Asset asset = Program.Services.New(testType, Oid.FromToken(item.Id, Program.MetaModel));
                asset.Attributes.Remove("Test.Scope");
                asset.SetAttributeValue(testName, test.Name);
                asset.SetAttributeValue(testEstimate, test.Estimate);
                asset.SetAttributeValue(testTodo, test.Todo);
                asset.AddAttributeValue(testOwners, Oid.FromToken(owners[ownerIndex].Id, Program.MetaModel));
                ownerIndex = ownerIndex == 1 ? 2 : 1;

                Program.Services.Save(asset);
                test.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void UpdateTest(string testId, string status, int todo)
        {
            Query query = new Query(Oid.FromToken(testId, Program.MetaModel));
            IAssetType testType = Program.MetaModel.GetAssetType("Test");
            IAttributeDefinition statusAttribute = testType.GetAttributeDefinition("Status");
            IAttributeDefinition toDoAttribute = testType.GetAttributeDefinition("ToDo");
            QueryResult result = Program.Services.Retrieve(query);
            Asset test = result.Assets[0];
            test.SetAttributeValue(statusAttribute, Oid.FromToken(status, Program.MetaModel));
         
            if (todo > -1)
                test.SetAttributeValue(toDoAttribute, todo);
            
            Program.Services.Save(test);
        }
        #endregion
    }
}
