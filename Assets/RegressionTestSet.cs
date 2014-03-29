using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class RegressionTestSet
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public int Estimate { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public RegressionTestSet(string name, string priority, string status, int estimate)
        {
            Name = name;
            Priority = priority;
            Status = status;
            Estimate = estimate;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddReleaseTestSets(RegressionSuite regressionSuite)
        {
            regressionSuite.RegressionTestSets.Add(new RegressionTestSet("Regression Tests for sprint 1", Utils.PriorityHigh, Utils.StatusNone, 1));
        }

        public static void SaveTestSets(RegressionSuite suite, Project project, IList<Member> owners)
        {
            IAssetType testSetType = Program.MetaModel.GetAssetType("TestSet");
            IAttributeDefinition testSetName = testSetType.GetAttributeDefinition("Name");
            IAttributeDefinition testSetRegressionSuite = testSetType.GetAttributeDefinition("RegressionSuite");
            IAttributeDefinition testSetPriority = testSetType.GetAttributeDefinition("Priority");
            IAttributeDefinition testSetStatus = testSetType.GetAttributeDefinition("Status");
            IAttributeDefinition testSetEstimate = testSetType.GetAttributeDefinition("Estimate");
            IAttributeDefinition testSetEnvironment = testSetType.GetAttributeDefinition("Environment");
            IAttributeDefinition testSetIteration = testSetType.GetAttributeDefinition("Timebox");
            IAttributeDefinition testSetOwners = testSetType.GetAttributeDefinition("Owners");

            int randomForEnvironment = 1;
            foreach (RegressionTestSet testSet in suite.RegressionTestSets)
            {
                //Asset asset = Program.Services.New(testSetType, Oid.FromToken(project.Id, Program.MetaModel));
                Asset asset = Program.Services.New(testSetType, Oid.FromToken(project.Releases[0].Id, Program.MetaModel));
                asset.SetAttributeValue(testSetName, testSet.Name);
                asset.SetAttributeValue(testSetRegressionSuite, Oid.FromToken(suite.Id, Program.MetaModel));
                asset.SetAttributeValue(testSetPriority, Oid.FromToken(testSet.Priority, Program.MetaModel));
                asset.SetAttributeValue(testSetEstimate, testSet.Estimate);

                if (project.Environments.Count > 0)
                {
                    randomForEnvironment %= project.Environments.Count;
                    asset.SetAttributeValue(testSetEnvironment, project.Environments[randomForEnvironment].Id);
                }

                //if ( testSet.Iteration != null )
                //{
                    //asset.SetAttributeValue( testSetIteration, Oid.FromToken( testSet.Iteration.Id, Program.MetaModel ) );
                    asset.SetAttributeValue(testSetIteration, Oid.FromToken(project.Iterations[0].Id, Program.MetaModel));

                    // Only add owner if story is in iteration
                    asset.AddAttributeValue(testSetOwners, Oid.FromToken(owners[0].Id, Program.MetaModel));
                //}

                if (testSet.Status != Utils.StatusNone)
                    asset.SetAttributeValue(testSetStatus, Oid.FromToken(testSet.Status, Program.MetaModel));

                Program.Services.Save(asset);
                testSet.Id = asset.Oid.Momentless.Token;
                randomForEnvironment++;

                IOperation copyTest = Program.MetaModel.GetOperation("TestSet.CopyAcceptanceTestsFromRegressionSuite");
                Oid testSetOid = Program.Services.ExecuteOperation(copyTest, Oid.FromToken( testSet.Id, Program.MetaModel));

                // Update owner, detail estimate, todo and status for all TestSet tests in an iteration
                //if ( testSet.Iteration != null )
                //    UpdateIterationTests( testSetOid, owners );
            }
        }

        //private static void UpdateIterationTests( Oid testSetOid, IList<Member> owners )
        //{
        //    IAssetType testType = Program.MetaModel.GetAssetType( "Test" );

        //    Query query = new Query( testType );

        //    IAttributeDefinition parentDefn = testType.GetAttributeDefinition( "Parent" );
        //    IAttributeDefinition ownersDefn = testType.GetAttributeDefinition( "Owners" );
        //    IAttributeDefinition detailEstDefn = testType.GetAttributeDefinition( "DetailEstimate" );
        //    IAttributeDefinition todoDefn = testType.GetAttributeDefinition( "ToDo" );
        //    IAttributeDefinition statusDefn = testType.GetAttributeDefinition( "Status" );
        //    IAttributeDefinition parentStatusDefn = testType.GetAttributeDefinition( "Parent.Status" );

        //    query.Selection.Add( ownersDefn );
        //    query.Selection.Add( detailEstDefn );
        //    query.Selection.Add( todoDefn );
        //    query.Selection.Add( statusDefn );
        //    query.Selection.Add( parentStatusDefn );

        //    FilterTerm filter = new FilterTerm( parentDefn );
        //    filter.Equal( testSetOid );
        //    query.Filter = filter;

        //    QueryResult result = Program.Services.Retrieve( query );

        //    foreach ( Asset test in result.Assets )
        //    {
        //        test.AddAttributeValue( ownersDefn, Oid.FromToken( owners[ 1 ].Id, Program.MetaModel ) );
        //        test.SetAttributeValue( detailEstDefn, 1 );

        //        string testSetStatus = test.GetAttribute( parentStatusDefn ).Value.ToString();

        //        if ( testSetStatus == Utils.StatusDone || testSetStatus == Utils.StatusAccepted )
        //        {
        //            test.SetAttributeValue( todoDefn, 0 );
        //            test.SetAttributeValue( statusDefn, Utils.TestStatusPassed );
        //        }
        //        else
        //        {
        //            test.SetAttributeValue( todoDefn, 1 );
        //        }

        //        Program.Services.Save( test );
        //    }
        //}
        #endregion
    }
}
