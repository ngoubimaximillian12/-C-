using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Petition.Models
{
    public class Member : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public ICollection<Petition_signature> petition_Signatures { get; set; }
        public string Role { get; set; } = "Member";
        public bool IsBlocked { get;set; }
    }
}
