using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    //This class creates Requests in the Product Backlog only (and not the Release project) under the reasoning that product owner gets all 
    //requests in the product backlog and decides the merit of the request and it's disposition as far as releases.
    public class Request
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PriorityId { get; set; }
        public string StatusId { get; set; }
        public string RequestedBy { get; set; }
        #endregion

        #region "CONSTRCUTORS"
        public Request(string name, string description, string priorityId, string statusId, string requestedBy)
        {
            Name = name;
            Description = description;
            PriorityId = priorityId;
            StatusId = statusId;
            RequestedBy = requestedBy;
        }
        #endregion

        #region "STATIC METHODS"

        public static void AddProductRequests(Project project)
        {
            project.Requests.Add(new Request("Total Usage Report", "", Utils.RequestPriorityHigh, Utils.RequestStatusApproved, "customer"));
            project.Requests.Add(new Request("Integrate with Palm Handheld", "", Utils.RequestPriorityLow, Utils.RequestStatusReviewed, "sales"));
            project.Requests.Add(new Request("Call History Calendar", "", Utils.RequestPriorityMedium, Utils.RequestStatusApproved, "customer"));
        }

        public static void SaveRequests(Project project)
        {
            IAssetType requestType = Program.MetaModel.GetAssetType("Request");
            IAttributeDefinition nameDef = requestType.GetAttributeDefinition("Name");
            IAttributeDefinition descDef = requestType.GetAttributeDefinition("Description");
            IAttributeDefinition priorityDef = requestType.GetAttributeDefinition("Priority");
            IAttributeDefinition statusDef = requestType.GetAttributeDefinition("Status");
            IAttributeDefinition requestedByDef = requestType.GetAttributeDefinition("RequestedBy");

            foreach (Request request in project.Requests)
            {
                Asset asset = Program.Services.New(requestType, Oid.FromToken( project.Id, Program.MetaModel));
                asset.SetAttributeValue(nameDef, request.Name);
                asset.SetAttributeValue(descDef, request.Description);
                asset.SetAttributeValue(priorityDef, Oid.FromToken(request.PriorityId, Program.MetaModel));
                asset.SetAttributeValue(statusDef, Oid.FromToken(request.StatusId, Program.MetaModel));
                asset.SetAttributeValue(requestedByDef, request.RequestedBy);

                Program.Services.Save(asset);
                request.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void GenerateStoryFromRequest(Project project)
        {
            if (project.Requests.Count > 0)
            {
                Oid requestOid = Oid.FromToken(project.Requests[0].Id, Program.MetaModel);
                IAssetType storyType = Program.MetaModel.GetAssetType("Story");
                Asset newStory = Program.Services.New(storyType, requestOid);
                Program.Services.Save(newStory);
            }
        }
        #endregion
    }
}
