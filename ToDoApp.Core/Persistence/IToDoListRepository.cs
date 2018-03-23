using System.Collections.Generic;
using ToDoApp.Entity.Model;
using ToDoApp.Entity.SearchArgs;

namespace ToDoApp.Core.Persistence
{
    public interface IToDoListRepository : IMasterRepository<ToDoList, int>
    {
        IList<ToDoList> Search(ToDoListSearchArgs args);
        ToDoList Save(ToDoList toDoList);
        bool Update(ToDoList toDoList);
        bool Delete(int id);
        IList<ToDoList> GetNotificationNotSendItems();
    }
}
