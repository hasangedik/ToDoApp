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
    public class TaskRepository : MasterRepository<Task, int>, ITaskRepository
    {
        private readonly DbSet<Task> _dbSet;
        public TaskRepository(UnitOfWork unitOfWork) : base(unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException();

            _dbSet = unitOfWork.DatabaseContext.Set<Task>();
        }

        public IList<Task> Search(TaskSearchArgs args)
        {
            if (args == null)
                return null;

            var result = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(args.Title))
                result = result.Where(x => x.Title == args.Title);

            if (args.ToDoListId.HasValue)
            {
                result = result.Where(x => x.ToDoListId == args.ToDoListId.Value);
            }

            return result.ToList();
        }

        public Task Save(Task task)
        {
            var entity = _dbSet.Add(task);
            return entity;
        }

        public bool Update(Task task)
        {
            _dbSet.AddOrUpdate(task);
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
