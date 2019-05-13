using DataVisualization.Domain.Contracts;
using DataVisualization.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataVisualization.Domain.Services
{
    public abstract class BaseDomain<TId, TEntity> : IBaseDomain<TId, TEntity>
        where TEntity : class
    {
        protected readonly IBaseRepository<TId,TEntity> repository;

        public BaseDomain(IBaseRepository<TId, TEntity> repository)
        {
            this.repository = repository;
        }
        public virtual TEntity Add(TEntity entity)
        {
            return repository.Add(entity);
        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            return repository.AddRange(entities);
        }

        public virtual bool Delete(TId id)
        {
            return repository.Delete(id);
        }

        public virtual bool Delete(TEntity entityToDelete)
        {
            return repository.Delete(entityToDelete);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, int skipAmount, int takeAmount, out int totalCount)
        {
            return repository.Get(filter, orderBy, skipAmount, takeAmount, out totalCount);
        }

        public IEnumerable<TEntity> Get(
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
           int skipAmount,
           int takeAmount,
           out int totalCount
          )
        {
            return repository.Get(orderBy, skipAmount, takeAmount, out totalCount);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return repository.Get(filter, orderBy, includeProperties);
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter, string orderBy, int skipAmount, int takeAmount, out int totalCount)
        {
            return repository.Get(filter, orderBy, skipAmount, takeAmount, out totalCount);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return repository.GetAll();
        }

        public virtual TEntity GetById(TId id)
        {
            return repository.GetById(id);
        }

        public virtual int Save()
        {
            return repository.Save();
        }

        public virtual TEntity Update(TEntity entity)
        {
            return repository.Update(entity);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    repository.Dispose();
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
