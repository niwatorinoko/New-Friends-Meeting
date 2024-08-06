using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PostId { get; set; }

        [Required]
        public int PlayerId { get; set; }

        [MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [MaxLength(20)]
        public string Place { get; set; }

        public DateTime? MeetingDateTime { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player? Player { get; set; }
    }
}
