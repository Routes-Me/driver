using driver_service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace driver_service.Abstraction
{
    public interface IGenericRepository<T> where T : class
    {
        List<T> Get(Pagination pagination, Expression<Func<T, bool>> filter = null,
                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                    params Expression<Func<T, object>>[] includeProperties);

        T GetById(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties);
        void Post(T entity);
        void Remove(T entity);
    }
}
