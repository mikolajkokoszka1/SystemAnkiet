using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Treść odpowiedzi jest wymagana")]
        [StringLength(300, ErrorMessage = "Odpowiedź nie może przekraczać 300 znaków")]
        [Column(TypeName = "nvarchar(300)")]
        public string AnswerText { get; set; }

        [Required(ErrorMessage = "Kolejność jest wymagana")]
        public int Order { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }

        public ICollection<ResponseDetail>? ResponseDetails { get; set; }
    }
}
