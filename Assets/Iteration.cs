using System;
using System.Collections.Generic;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Iteration
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string StateId { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Target { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Iteration(string name, string stateId, DateTime beginDate, DateTime endDate)
        {
            Name = name;
            StateId = stateId;
            BeginDate = beginDate;
            EndDate = endDate;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddIterations(Project project)
        {
            switch (Program._v1ClientTarget)
            {
                case "CapitalOne":
                {
                    //Iteration 1
                    DateTime beginDate = Utils.GetTodaysDateNoTime().AddDays(-2);
                    DateTime endDate = Utils.GetTodaysDateNoTime().AddDays(12);
                    Iteration iteration = new Iteration("Sprint 1", "State:101", beginDate, endDate) { Target = 14 };
                    project.Iterations.Add(iteration);

                    //Iteration 2
                    beginDate = endDate;
                    endDate = beginDate.AddDays(14);
                    iteration = new Iteration("Sprint 2", "State:101", beginDate, endDate) { Target = 14 };
                    project.Iterations.Add(iteration);

                    //Iteration 3
                    beginDate = endDate;
                    endDate = beginDate.AddDays(14);
                    iteration = new Iteration("Sprint 3", "State:100", beginDate, endDate) { Target = 14 };
                    project.Iterations.Add(iteration);
                    break;
                }

                default:
                {
                    //Iteration 1
                    DateTime beginDate = Utils.GetTodaysDateNoTime();
                    DateTime endDate = Utils.GetTodaysDateNoTime().AddDays(7);
                    Iteration iteration = new Iteration("Iteration 1", "State:101", beginDate, endDate) { Target = 16 };
                    project.Iterations.Add(iteration);

                    //Iteration 2
                    beginDate = endDate;
                    endDate = beginDate.AddDays(7);
                    iteration = new Iteration("Iteration 2", "State:100", beginDate, endDate) { Target = 16 };
                    project.Iterations.Add(iteration);

                    //Iteration 3
                    beginDate = endDate;
                    endDate = beginDate.AddDays(7);
                    iteration = new Iteration("Iteration 3", "State:100", beginDate, endDate) { Target = 16 };
                    project.Iterations.Add(iteration);
                    break;
                }
            }
        }

        public static void SaveIterations(Project project)
        {
            IAssetType iterationType = Program.MetaModel.GetAssetType("Timebox");
            IAttributeDefinition iterationName = iterationType.GetAttributeDefinition("Name");
            IAttributeDefinition iterationState = iterationType.GetAttributeDefinition("State");
            IAttributeDefinition iterationBeginDate = iterationType.GetAttributeDefinition("BeginDate");
            IAttributeDefinition iterationEndDate = iterationType.GetAttributeDefinition("EndDate");
            IAttributeDefinition targetEstimate = iterationType.GetAttributeDefinition("TargetEstimate");
            IList<Iteration> iterations = project.Iterations;

            foreach (Iteration iteration in iterations)
            {
                //shouldn't have to do this - assetState should not be sent in initialized object when calling services.New
                Asset asset = Program.Services.New(iterationType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.Attributes.Remove("Timebox.AssetState");
                asset.SetAttributeValue(iterationName, iteration.Name);
                asset.SetAttributeValue(iterationState, Oid.FromToken(iteration.StateId, Program.MetaModel));
                asset.SetAttributeValue(iterationBeginDate, iteration.BeginDate);
                asset.SetAttributeValue(iterationEndDate, iteration.EndDate);
                asset.SetAttributeValue(targetEstimate, iteration.Target);

                Program.Services.Save(asset);
                iteration.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void SaveDefaultIterations(Project project)
        {
            //Check if the default iteration has already been created.
            IAssetType assetType = Program.MetaModel.GetAssetType("Timebox");
            Query query = new Query(assetType);
            IAttributeDefinition nameAttribute = assetType.GetAttributeDefinition("Name");
            FilterTerm term = new FilterTerm(nameAttribute);
            term.Equal("Iteration 1");
            query.Filter = term;
            QueryResult result = Program.Services.Retrieve(query);

            //If there is no iteration, create one and store it with project.
            if (result.Assets.Count == 0)
            {
                IAssetType iterationType = Program.MetaModel.GetAssetType("Timebox");
                IAttributeDefinition iterationName = iterationType.GetAttributeDefinition("Name");
                IAttributeDefinition iterationState = iterationType.GetAttributeDefinition("State");
                IAttributeDefinition iterationBeginDate = iterationType.GetAttributeDefinition("BeginDate");
                IAttributeDefinition iterationEndDate = iterationType.GetAttributeDefinition("EndDate");
                IAttributeDefinition targetEstimate = iterationType.GetAttributeDefinition("TargetEstimate");
                IList<Iteration> iterations = project.Iterations;

                foreach (Iteration iteration in iterations)
                {
                    //shouldn't have to do this - assetState should not be sent in initialized object when calling services.New
                    Asset asset = Program.Services.New(iterationType, Oid.FromToken(project.Id, Program.MetaModel));
                    asset.Attributes.Remove("Timebox.AssetState");
                    asset.SetAttributeValue(iterationName, iteration.Name);
                    asset.SetAttributeValue(iterationState, Oid.FromToken(iteration.StateId, Program.MetaModel));
                    asset.SetAttributeValue(iterationBeginDate, iteration.BeginDate);
                    asset.SetAttributeValue(iterationEndDate, iteration.EndDate);
                    asset.SetAttributeValue(targetEstimate, iteration.Target);

                    Program.Services.Save(asset);
                    iteration.Id = asset.Oid.Momentless.Token;
                }
            }

            //Found the default iteration, so store that with the project.
            else
            {
                foreach (Iteration iteration in project.Iterations)
                {
                    iteration.Id = result.Assets[0].Oid.Momentless.Token;
                }
            }
        }
        #endregion
    }
}
