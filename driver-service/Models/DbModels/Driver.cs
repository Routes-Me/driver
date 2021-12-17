
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DriverService.Models.DBModels
{
    public partial class Driver
    {
        public int DriverId { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string InvitationToken { get; set; }

        public virtual Phone Phone { get; set; }
    }
}
