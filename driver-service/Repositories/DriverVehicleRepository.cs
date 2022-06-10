using driver_service.Abstraction;
using driver_service.Models;
using driver_service.Models.Entities;

namespace driver_service.Repositories
{
    public class DriverVehicleRepository : GenericRepository<DriverVehicle>, IDriverVehicleRepository
    {
        public DriverVehicleRepository(DriverContext context) : base(context)
        {
        }
    }
}
