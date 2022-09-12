using System;

namespace driver_service.Models.DTO
{
    public class DriverDeviceDto
    {
        public string DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public string SimSerialNumber { get; set; }
        public string VehicleId { get; set; }
        public DateTime CreatedAt { get; set; }
        //public DeviceVehicleDto DeviceVehicleDto { get; set; }
    }
}
