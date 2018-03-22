using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ToDoApp.Core.Persistence;

namespace ToDoApp.Infrastructure.Persistence
{
    public class MasterRepository<TEntity, TKey> : IMasterRepository<TEntity, TKey> where TEntity : class
    {
        private readonly DbSet<TEntity> _dbSet;

        protected MasterRepository(IUnitOfWork<DbContext> unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException(nameof(unitOfWork), @"Unit of work cannot be null");
            
            _dbSet = unitOfWork.DatabaseContext.Set<TEntity>();
        }

        public TEntity Get(TKey id)
        {
            return _dbSet.Find(id);
        }

        public async Task<TEntity> GetAsnyc(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.ToList();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate).ToList();
        }

        protected TEntity SetEntityFields<T>(TEntity entity, TEntity from, params string[] properties)
        {
            var type = entity.GetType();
            
            foreach (string field in properties)
            {
                if (type.GetProperty(field) == null)
                    throw new ArgumentException();
                var propertyInfo = type.GetProperty(field);
                if (propertyInfo == null)
                    continue;

                propertyInfo.SetValue(entity, propertyInfo.GetValue(@from, null), null);
            }
            return entity;
        }
    }
}
