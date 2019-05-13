using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataVisualization.Domain.Contracts
{
    public interface IBaseDomain<TId,TEntity> : IDisposable
    {
        IEnumerable<TEntity> GetAll();

        TEntity GetById(TId id);

        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
           int skipAmount,
           int takeAmount,
           out int totalCount
          );

        IEnumerable<TEntity> Get(
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
           int skipAmount,
           int takeAmount,
           out int totalCount
          );

        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "");
        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter,
           string orderBy,
             int skipAmount,
           int takeAmount,
           out int totalCount
          );

        TEntity Add(TEntity entity);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);

        TEntity Update(TEntity entity);

        bool Delete(TId id);

        int Save();

        bool Delete(TEntity entityToDelete);
    }
}
