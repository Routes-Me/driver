using driver_service.Abstraction;
using driver_service.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace driver_service.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal DriverContext _context;
        public DbSet<T> dbSet;

        public GenericRepository(DriverContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }

        public void Delete(int id)
        {
            T entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }
        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return dbSet.SingleOrDefault(predicate);
        }
        public void Delete(T entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await dbSet.Where(predicate).ToListAsync();
        }

        public List<T> Get(Pagination pagination, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = dbSet;
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
                foreach (Expression<Func<T, object>> includeProperty in includeProperties)
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
            IQueryable<T> query = dbSet;

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

        public T GetById(int id)
        {
            return dbSet.Find(id);
            //var res = dbSet.Find(id) as List<T>;
            //return res.FirstOrDefault();
        }

        public void Post(T entity)
        {
            dbSet.Add(entity);
        }

        public void Put(T entity)
        {
            dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
        public void Remove(T entity)
        {
            _context.Entry(entity).State = EntityState.Deleted;
        }

        public List<T> Get(Pagination pagination)
        {
            return dbSet.ToList();
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
