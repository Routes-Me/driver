using driver_service.Abstraction;
using driver_service.Models;
using driver_service.Models.Entities;
using System;

namespace driver_service.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DriverContext _context;
        public IDriverRepository DriverRepository { get; private set; }
        public IDriverVehicleRepository DriverVehicleRepository { get; private set; }


        private bool disposed = false;
        public UnitOfWork(DriverContext context)
        {
            _context = context;

            if (DriverRepository is null)
            {
                DriverRepository = new DriverRepository(_context);

            }
            if (DriverVehicleRepository is null)
            {
                DriverVehicleRepository = new DriverVehicleRepository(_context);

            }
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _context.Database.CommitTransaction();
            _context.Dispose();
        }
        public void Rollback()
        {
            _context.Database.RollbackTransaction();
            _context.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
