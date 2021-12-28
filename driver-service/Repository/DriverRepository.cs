using driver_service.Repository;
using DriverService.Abstraction;
using DriverService.Models.DBModels;

namespace DriverService.Repository
{
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {


    }
}