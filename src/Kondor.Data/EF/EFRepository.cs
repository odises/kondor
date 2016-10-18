using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Kondor.Domain;

namespace Kondor.Data.EF
{
    public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal IDbContext DbContext;
        internal DbSet<TEntity> DbSet;

        public EFRepository(IDbContext context)
        {
            DbContext = context;
            DbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public bool Any(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Any();
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.FirstOrDefault();
        }

        public IEnumerable<IGrouping<TKey, TEntity>> FilterThenGroupBy<TKey>(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TKey>> keySelector)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.GroupBy(keySelector);
        }

        public IEnumerable<IGrouping<TKey, TEntity>> GroupBy<TKey>(Expression<Func<TEntity, TKey>> keySelector)
        {
            IQueryable<TEntity> query = DbSet;

            return query.GroupBy(keySelector);
        }

        public virtual TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }

        public IEnumerable<TEntity> Random(Expression<Func<TEntity, bool>> filter = null, int count = 0)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = query.OrderBy(p => Guid.NewGuid());
            if (count > 0)
            {
                query = query.Take(count);
            }

            return query;
        }

        public void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            if (DbContext.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }

        public void Delete(object id)
        {
            var entity = GetById(id);
            Delete(entity);
        }

        public TEntity Create()
        {
            return DbSet.Create();
        }
    }
}
