using DataVisualization.Context;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace DataVisualization.Repository.Services
{
    public abstract class BaseRepository<TId, TEntity> : IBaseRepository<TId, TEntity> where TEntity : class
    {
        protected readonly VisContext context;
        protected readonly DbSet<TEntity> _entities;

        public BaseRepository(VisContext context)
        {
            this.context = context;
            this._entities = context.Set<TEntity>();

        }

        public virtual TEntity Add(TEntity entity)
        {
            var retVal = _entities.Add(entity);
            context.SaveChanges();
            return retVal;

        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entity)
        {
            var retVal = _entities.AddRange(entity);
            context.SaveChanges();
            return retVal;

        }

        public virtual bool Delete(TId id)
        {
            _entities.Remove(_entities.Find(id));
            var retValue = context.SaveChanges();
            return retValue == 1;
        }

        public virtual bool Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _entities.Attach(entityToDelete);
            }
            _entities.Remove(entityToDelete);
            var retValue = Save();
            return retValue == 1;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return _entities;
        }

        public virtual TEntity GetById(TId id)
        {
            return _entities.Find(id);
        }

        public virtual IEnumerable<TEntity> Get(
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
           int skipAmount,
           int takeAmount,
           out int totalCount
          )
        {
            IQueryable<TEntity> query = _entities;

            query = orderBy(query);
            totalCount = query.Count();
            query = query.Skip(skipAmount).Take(takeAmount);
            return query.ToList();
        }

        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
           int skipAmount,
           int takeAmount,
           out int totalCount
          )
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            totalCount = query.Count();
            query = orderBy(query);
            query = query.Skip(skipAmount).Take(takeAmount);
            return query.ToList();
        }

        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter,
           string orderBy,
           int skipAmount,
           int takeAmount,
           out int totalCount
          )
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalCount = query.Count();

            query = query.OrderBy(orderBy);

            query = query.Skip(skipAmount).Take(takeAmount);
            
            return query.ToList();
        }

        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = _entities;

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

        public virtual TEntity Update(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Detached)
                _entities.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
            var retVal = Save();
            return entity;
        }

        public virtual int Save()
        {
            return context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
