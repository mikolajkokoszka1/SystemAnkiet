using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Treść pytania jest wymagana")]
        [StringLength(500, ErrorMessage = "Pytanie nie może przekraczać 500 znaków")]
        [Column(TypeName = "nvarchar(500)")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Typ pytania jest wymagany")]
        [Column(TypeName = "nvarchar(50)")]
        public string QuestionType { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool IsRequired { get; set; } = true;

        [Required]
        public int SurveyId { get; set; }

        [ForeignKey("SurveyId")]
        public Survey? Survey { get; set; }

        public ICollection<Answer>? Answers { get; set; }

        public ICollection<ResponseDetail>? ResponseDetails { get; set; }
    }
}
