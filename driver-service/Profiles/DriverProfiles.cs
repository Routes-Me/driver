using AutoMapper;
using driver_service.Models.ResponseModel;
using DriverService.Models.DBModels;
using DriverService.Models.ResponseModel;

namespace driver_service.Profiles
{
    public class DriverProfiles : Profile
    {
        public DriverProfiles()
        {
            //Read Dto's
            CreateMap<Driver, DriversDto>();
            CreateMap<Driver, DriversReadDto>();
            CreateMap<DriversDto, Driver>();
        }
    }
}
