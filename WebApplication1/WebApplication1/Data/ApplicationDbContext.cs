using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseDetail> ResponseDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Response>()
                .HasOne(r => r.Survey)
                .WithMany(s => s.Responses)
                .HasForeignKey(r => r.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResponseDetail>()
                .HasOne(rd => rd.Response)
                .WithMany(r => r.ResponseDetails)
                .HasForeignKey(rd => rd.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ResponseDetail>()
                .HasOne(rd => rd.Question)
                .WithMany(q => q.ResponseDetails)
                .HasForeignKey(rd => rd.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ResponseDetail>()
                .HasOne(rd => rd.Answer)
                .WithMany(a => a.ResponseDetails)
                .HasForeignKey(rd => rd.AnswerId)
                .OnDelete(DeleteBehavior.NoAction);
        }


    }
}
