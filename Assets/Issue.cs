using System;
using System.Collections.Generic;
using System.Linq;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Issue
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime TargetDate { get; set; }
        public string PriorityId { get; set; }
        public string TypeId { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Issue(string name, DateTime targetDate, string priorityId, string typeId)
        {
            Name = name;
            PriorityId = priorityId;
            TargetDate = targetDate;
            TypeId = typeId;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddReleaseIssues(Project project)
        {
            project.Issues.Add(new Issue("No UPS Devices Left", DateTime.Now.AddDays(10), Utils.IssuePriorityHigh, Utils.IssueTypeImpediment));
            project.Issues.Add(new Issue("DBA Team Training", DateTime.Now.AddDays(-5), Utils.IssuePriorityLow, Utils.IssueTypeImpediment));
            project.Issues.Add(new Issue("No Hardware Yet", DateTime.Now.AddDays(3), Utils.IssuePriorityHigh, Utils.IssueTypeImpediment));
        }

        public static void SaveIssues(Project project, IList<Member> owners)
        {
            IAssetType issueType = Program.MetaModel.GetAssetType("Issue");
            IAttributeDefinition nameDef = issueType.GetAttributeDefinition("Name");
            IAttributeDefinition targetDateDef = issueType.GetAttributeDefinition("TargetDate");
            IAttributeDefinition priorityDef = issueType.GetAttributeDefinition("Priority");
            IAttributeDefinition typeDef = issueType.GetAttributeDefinition("Category");
            IAttributeDefinition ownersDef = issueType.GetAttributeDefinition("Owner");

            foreach (Issue issue in project.Issues)
            {
                Asset asset = Program.Services.New(issueType, Oid.FromToken(project.Id, Program.MetaModel));
                asset.SetAttributeValue(nameDef, issue.Name);
                asset.SetAttributeValue(targetDateDef, issue.TargetDate);
                asset.SetAttributeValue(priorityDef, Oid.FromToken(issue.PriorityId, Program.MetaModel));
                asset.SetAttributeValue(typeDef, Oid.FromToken(issue.TypeId, Program.MetaModel));
                asset.SetAttributeValue(ownersDef, Oid.FromToken(owners[0].Id, Program.MetaModel)); // can only have a single owner on an Issue

                Program.Services.Save(asset);
                issue.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void BlockDefectWithIssue(Project project)
        {
            if (project.Issues.Count > 0)
            {
                Query query = new Query(Oid.FromToken(project.Issues[0].Id, Program.MetaModel));
                IAttributeDefinition workitemsDef = Program.MetaModel.GetAttributeDefinition("Issue.BlockedPrimaryWorkitems");
                QueryResult result = Program.Services.Retrieve(query);
                Asset issue = result.Assets[0];

                string defectId = project.Defects.First(x => x.Name.Equals("Fix performance SLA on load")).Id;
                issue.AddAttributeValue(workitemsDef, Oid.FromToken(defectId, Program.MetaModel));
                Program.Services.Save(issue);
            }
        }
        #endregion
    }
}
