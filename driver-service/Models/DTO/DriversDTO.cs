using System.Collections.Generic;

namespace driver_service.Models.DTO
{
    public class DriversDTO
    {
        public string DriverId { get; set; }
        public string InvitationId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string VehicleId { get; set; }
    }
}
