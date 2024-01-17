using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Attendance.Models
{
    // Legacy User class (table)
    public class User : IdentityUser
    {
        [StringLength(128)]
        public string FirstName { get; set; }
        [StringLength(128)]
        public string LastName { get; set; }
    }
}