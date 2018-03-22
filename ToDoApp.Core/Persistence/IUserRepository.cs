using System.Collections.Generic;
using ToDoApp.Entity.Model;
using ToDoApp.Entity.SearchArgs;

namespace ToDoApp.Core.Persistence
{
    public interface IUserRepository: IMasterRepository<User, int>
    {
        User GetUserByEmail(string email);
        IList<User> Search(UserSearchArgs args);
        User Save(User user);
        bool Update(User user);
    }
}
