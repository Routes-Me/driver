namespace driver_service.Models.Entities
{
    public class DriverVehicle
    {
        public int DriverVehicleId { get; set; }
        public int? VehicleId { get; set; }
        public int DriverId { get; set; }
        public virtual Driver Driver { get; set; }
    }
}
