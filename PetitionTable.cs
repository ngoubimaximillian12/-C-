using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Petition.Models
{
    public class PetitionTable
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Picture { get; set; } 

        public ICollection<Petition_signature> petition_Signatures { get; set; }
    }
}
