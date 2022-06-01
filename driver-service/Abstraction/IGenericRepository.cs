using driver_service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace driver_service.Abstraction
{
    public interface IGenericRepository<T> where T : class
    {
        void Delete(int id);

        List<T> Get(Pagination pagination, Expression<Func<T, bool>> filter = null,
                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                    params Expression<Func<T, object>>[] includeProperties);

        List<T> Get(Pagination pagination);

        T GetById(int id);
        T GetById(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        void Post(T entity);
        void Put(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Remove(T entity);

        void Save();
    }
}
