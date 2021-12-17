using driver_service.Models.DbModels;
using driver_service.Models.ResponseModel;
using AutoMapper;

namespace driver_service.Profiles
{
    public class DriverProfiles : Profile
    {
        public DriverProfiles()
        {
            CreateMap<DeviceDto, Device>();
        }
    }
}
