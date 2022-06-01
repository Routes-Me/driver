using System.Collections.Generic;

namespace driver_service.Models.DTO
{
    public class DriversDTO
    {
        public string DriverId { get; set; }
        public string InvitationId { get; set; }
        public string UserId { get; set; }
        public List<string> VehicleId { get; set; } = new List<string>();

    }

    public class DriverVehiclesDTO
    {
        public string VehicleId { get; set; } 
    }
}
