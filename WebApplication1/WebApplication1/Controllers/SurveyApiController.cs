using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SurveyApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Survey>>> GetSurveys()
        {
            return await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Survey>> GetSurvey(int id)
        {
            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            return survey;
        }

        [HttpPost]
        public async Task<ActionResult<Survey>> PostSurvey(Survey survey)
        {
            survey.CreatedDate = DateTime.Now;
            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSurvey(int id, Survey survey)
        {
            if (id != survey.Id)
            {
                return BadRequest();
            }

            _context.Entry(survey).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SurveyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
    }
}
