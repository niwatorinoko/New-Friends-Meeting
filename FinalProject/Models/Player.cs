using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        [Required]
        public string PlayerName { get; set; }

        public string City { get; set; }

        public int Sex { get; set; }

        public DateTime Birthday { get; set; }

        public string Email { get; set; }

        [Required]
        public string Photo { get; set; }

        [Required]
        public string PlayerPassword { get; set; }

    }
}
