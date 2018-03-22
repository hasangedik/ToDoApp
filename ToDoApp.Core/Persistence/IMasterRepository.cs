using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ToDoApp.Core.Persistence
{
    public interface IMasterRepository<TEntity, in TKey> where TEntity : class
    {
        TEntity Get(TKey id);
        Task<TEntity> GetAsnyc(TKey id);
        IEnumerable<TEntity> GetAll();
        Task<IEnumerable<TEntity>> GetAllAsync();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    }
}
