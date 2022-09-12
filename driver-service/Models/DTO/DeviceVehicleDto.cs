using System;

namespace driver_service.Models.DTO
{
    public class DeviceVehicleDto
    {
        public string VehicleId { get; set; }
        public string PlateNumber { get; set; }
        public string InstitutionId { get; set; }
        public string ModelYear { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ModelId { get; set; }
    }
}
