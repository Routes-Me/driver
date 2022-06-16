﻿using driver_service.Abstraction;
using driver_service.Models;
using driver_service.Models.Entities;

namespace driver_service.Repositories
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        public DriverRepository(DriverContext context) : base(context)
        {
        }
    }
}