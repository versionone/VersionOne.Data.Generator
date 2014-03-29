using System.Collections.Generic;

namespace VersionOne.Data.Generator.Interfaces
{
    //Interface shared by objects that need to act-like-a workitem.
    public interface IWorkitem
    {
        IList<Test> Tests { get; set; }
        IList<Task> Tasks { get; set; }
        string Id { get; set; }
    }
}
