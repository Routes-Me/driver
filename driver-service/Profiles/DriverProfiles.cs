using AutoMapper;
using driver_service.Models.DbModels;
using driver_service.Models.ResponseModel;

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
