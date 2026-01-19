using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ResponseDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResponseId { get; set; }

        [ForeignKey("ResponseId")]
        public Response? Response { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        public int? AnswerId { get; set; }

        [ForeignKey("AnswerId")]
        public Answer? Answer { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string? TextAnswer { get; set; }
    }
}
