using System.Collections.Generic;
using ToDoApp.Entity.Model;
using ToDoApp.Entity.SearchArgs;

namespace ToDoApp.Core.Persistence
{
    public interface ITaskRepository : IMasterRepository<Task, int>
    {
        IList<Task> Search(TaskSearchArgs args);
        Task Save(Task task);
        bool Update(Task task);
    }
}
