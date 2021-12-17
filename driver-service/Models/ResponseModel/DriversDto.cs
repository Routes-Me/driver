
using DriverService.Models.DBModels;

namespace DriverService.Models.ResponseModel
{
    public class DriversDto
    {
        public string DriverId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public Phones phone { get; set; }
        public string AvatarUrl { get; set; }
        public string InvitationToken { get; set; }


        public class Phones
        {
            public string Number { get; set; }
            public string VerificationToken { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
