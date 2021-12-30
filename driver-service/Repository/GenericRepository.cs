using driver_service.Abstraction;
using DriverService.Models.DBModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace driver_service.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public DriversServiceContext _context = null;
        public DbSet<T> dbSet = null;

        public GenericRepository()
        {
            this._context = new DriversServiceContext();
            dbSet = _context.Set<T>();
        }
        public GenericRepository(DriversServiceContext _context)
        {
            this._context = _context;
            dbSet = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return dbSet.ToList();
        }
        public T GetById(object id)
        {
            return dbSet.Find(id);
        }
        public void Insert(T obj)
        {
            dbSet.Add(obj);
        }
        public void Update(T obj)
        {
            dbSet.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object id)
        {
            T existing = dbSet.Find(id);
            dbSet.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
