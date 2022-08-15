using driver_service.Abstraction;
using driver_service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace driver_service.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal DriverContext Context;
        public DbSet<T> DbSet;

        public GenericRepository(DriverContext context)
        {
            Context = context;
            DbSet = Context.Set<T>();
        }

        public List<T> Get(Pagination pagination, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;
            if (pagination != null)
            {
                query = query.Skip((pagination.Offset - 1) * pagination.Limit).Take(pagination.Limit);
                pagination.Total = query.Count();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties!= null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }


            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query.ToList();
        }

        public T GetById(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault();
        }

        public void Post(T entity)
        {
            DbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
        }
    }
}
