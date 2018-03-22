using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using ToDoApp.Core.Persistence;
using ToDoApp.Entity.Model;
using ToDoApp.Entity.SearchArgs;

namespace ToDoApp.Infrastructure.Persistence
{
    public class ToDoListRepository : MasterRepository<ToDoList, int>, IToDoListRepository
    {
        private readonly DbSet<ToDoList> _dbSet;
        public ToDoListRepository(UnitOfWork unitOfWork) : base(unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException();

            _dbSet = unitOfWork.DatabaseContext.Set<ToDoList>();
        }

        public IList<ToDoList> Search(ToDoListSearchArgs args)
        {
            if (args == null)
                return null;

            var result = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(args.Title))
                result = result.Where(x => x.Title == args.Title);

            if (!args.IsChecked.HasValue)
                result = result.Where(x => x.IsChecked == args.IsChecked.Value);

            return result.ToList();
        }

        public ToDoList Save(ToDoList toDoList)
        {
            var entity = _dbSet.Add(toDoList);
            return entity;
        }

        public bool Update(ToDoList toDoList)
        {
            _dbSet.AddOrUpdate(toDoList);
            return true;
        }

        public bool Delete(int id)
        {
            var entity = _dbSet.FirstOrDefault(x => x.Id == id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }
    }
}
