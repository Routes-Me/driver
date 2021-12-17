using System.Threading.Tasks;
using DriverService.Models;
using DriverService.Models.ResponseModel;

namespace DriverService.Abstraction
{
    public interface IDriverRepository
    {
        dynamic DeleteDriver(string driverId);
        dynamic UpdateDriver(DriversDto driversDto);
        dynamic GetDriver(string driverId, Pagination pageInfo, string includeType);
        dynamic PostDriver(DriversDto driversDto);
    }
}
