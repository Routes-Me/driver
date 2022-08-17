using driver_service.Models;
using driver_service.Models.ResponseModel;
using System.Collections.Generic;

namespace driver_service.Abstraction
{
    public interface IDriverIncludedRepository
    {
        dynamic GetDriver(Pagination pageInfo, string include);
        dynamic GetUsersIncludedData(List<DriversModel> objDriversModelList);
        dynamic GetInstitutionsIncludedData(List<DriversModel> objDriversModelList);
    }
}
