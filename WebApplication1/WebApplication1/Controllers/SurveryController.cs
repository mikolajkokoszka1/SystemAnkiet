using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class SurveyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurveyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var surveysQuery = _context.Surveys.Include(s => s.Responses).Include(s => s.Questions).ThenInclude(q => q.Answers).AsQueryable();

            if (!User.IsInRole("Admin"))
            {
                surveysQuery = surveysQuery.Where(s =>
                    s.Questions.Any() && s.Questions.All(q => q.Answers.Count >= 2)
                );
            }

            var surveys = await surveysQuery.ToListAsync();

            return View(surveys);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(m => m.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,Description")] Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.CreatedDate = DateTime.Now;
                survey.CreatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(survey);
                await _context.SaveChangesAsync();

                return RedirectToAction("Manage", "Question", new { id = survey.Id });
            }
            return View(survey);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FindAsync(id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(survey);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedDate,CreatorId")] Survey survey)
        {
            if (id != survey.Id)
            {
                return NotFound();
            }

            if (survey.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(survey);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurveyExists(survey.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(survey);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.FirstOrDefaultAsync(m => m.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            return View(survey);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool SurveyExists(int id)
        {
            return _context.Surveys.Any(e => e.Id == id);
        }
    }
}
