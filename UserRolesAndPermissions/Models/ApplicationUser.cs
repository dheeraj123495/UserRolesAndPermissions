using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace UserRolesAndPermissions.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string? City { get; set; }
        public string? State { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
