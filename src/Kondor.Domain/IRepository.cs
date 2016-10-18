using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kondor.Domain
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");
        int Count(Expression<Func<TEntity, bool>> filter = null);
        bool Any(Expression<Func<TEntity, bool>> filter = null);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter = null);
        IEnumerable<IGrouping<TKey, TEntity>> FilterThenGroupBy<TKey>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TKey>> keySelector);
        IEnumerable<IGrouping<TKey, TEntity>> GroupBy<TKey>(Expression<Func<TEntity, TKey>> keySelector);
        TEntity GetById(object id);
        IEnumerable<TEntity> Random(Expression<Func<TEntity, bool>> filter = null, int count = 0);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(object id);
        TEntity Create();
    }
}
