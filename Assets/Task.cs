using System.Collections.Generic;
using VersionOne.Data.Generator.Interfaces;
using VersionOne.SDK.APIClient;

namespace VersionOne.Data.Generator
{
    public class Task
    {
        #region "PROPERTIES"
        public string Id { get; set; }
        public string Name { get; set; }
        public string StatusId { get; set; }
        public int Estimate { get; set; }
        public string OwnerId { get; set; }
        public int Todo { get; set; }
        #endregion

        #region "CONSTRUCTORS"
        public Task(string name, int estimate, int todo)
        {
            Name = name;
            Estimate = estimate;
            Todo = todo;
        }
        #endregion

        #region "STATIC METHODS"

        public static void SaveTasks(IWorkitem item, IList<Member> owners)
        {
            IAssetType taskType = Program.MetaModel.GetAssetType("Task");
            IAttributeDefinition taskName = taskType.GetAttributeDefinition("Name");
            IAttributeDefinition taskEstimate = taskType.GetAttributeDefinition("DetailEstimate");
            IAttributeDefinition taskOwners = taskType.GetAttributeDefinition("Owners");
            IAttributeDefinition taskTodo = taskType.GetAttributeDefinition("ToDo");

            IList<Task> tasks = item.Tasks;
            int ownerIndex = 1;

            foreach (Task task in tasks)
            {
                //shouldn't have to do this - scope should not be sent in initialized object when calling services.New
                Asset asset = Program.Services.New(taskType, Oid.FromToken(item.Id, Program.MetaModel));
                asset.Attributes.Remove("Task.Scope");
                asset.SetAttributeValue(taskName, task.Name);
                asset.SetAttributeValue(taskEstimate, task.Estimate);
                asset.SetAttributeValue(taskTodo, task.Todo);
                asset.AddAttributeValue(taskOwners, Oid.FromToken(owners[ownerIndex].Id, Program.MetaModel));
                ownerIndex = ownerIndex == 1 ? 2 : 1;

                Program.Services.Save(asset);
                task.Id = asset.Oid.Momentless.Token;
            }
        }

        public static void UpdateTask(string taskId, string status, int todo)
        {
            Query query = new Query(Oid.FromToken(taskId, Program.MetaModel));
            IAssetType taskType = Program.MetaModel.GetAssetType("Task");
            IAttributeDefinition statusAttribute = taskType.GetAttributeDefinition("Status");
            IAttributeDefinition toDoAttribute = taskType.GetAttributeDefinition("ToDo");
            QueryResult result = Program.Services.Retrieve(query);

            Asset task = result.Assets[0];
            task.SetAttributeValue(statusAttribute, Oid.FromToken(status, Program.MetaModel));

            if (todo > -1)
                task.SetAttributeValue(toDoAttribute, todo);

            Program.Services.Save(task);
        }
        #endregion
    }
}
