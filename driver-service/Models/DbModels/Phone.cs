using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace DriverService.Models.DBModels
{
    public partial class Phone
    {
        public int PhoneId { get; set; }

        public string Number { get; set; }
        public string VerificationToken { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }
        [ForeignKey("Driver")]
        public int DriverId { get; set; }

    }
}
