using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Survey
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł ankiety jest wymagany")]
        [StringLength(200, ErrorMessage = "Tytuł nie może przekraczać 200 znaków")]
        [Column(TypeName = "nvarchar(200)")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Opis nie może przekraczać 1000 znaków")]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(450)")]
        public string? CreatorId { get; set; }

        public ICollection<Question>? Questions { get; set; }

        public ICollection<Response>? Responses { get; set; }
    }
}
