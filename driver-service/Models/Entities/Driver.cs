using System;
using System.Collections.Generic;

namespace driver_service.Models.Entities
{
    public class Driver
    {
        public Driver()
        {
            DriverVehicles = new HashSet<DriverVehicle>();
        }
        public int DriverId { get; set; }
        public int InvitationId { get; set; }
        public int UserId { get; set; }

        public string Name { get; set; }
        public string AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<DriverVehicle> DriverVehicles { get; set; }
    }
}
