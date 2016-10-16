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
        TEntity GetById(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        void Delete(object id);
    }
}
