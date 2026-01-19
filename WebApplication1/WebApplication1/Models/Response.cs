using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Response
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Required]
        public int SurveyId { get; set; }

        [ForeignKey("SurveyId")]
        public Survey? Survey { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        public string? UserId { get; set; }

        public bool IsAnonymous { get; set; } = false;

        public ICollection<ResponseDetail>? ResponseDetails { get; set; }
    }
}
