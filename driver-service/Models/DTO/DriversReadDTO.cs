using System.Collections.Generic;

namespace driver_service.Models.DTO
{
    public class DriversReadDTO
    {
        public string DriverId { get; set; }
        public string UserId { get; set; }
        public string InvitationId { get; set; }
        public List<string> VehicleId { get; set; }
    }
}
