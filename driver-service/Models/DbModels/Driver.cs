using System;

namespace driver_service.Models.DbModels
{
    public partial class Driver
    {
        public int DriverId { get; set; }
        public int UserId { get; set; }
        public int InstitutionId { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
