using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace driver_service.Models.DbModels
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        public string fcmToken { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }
    }
}
