using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            ViewBag.SurveyId = id;
            ViewBag.SurveyTitle = survey.Title;

            return View(survey.Questions.OrderBy(q => q.Order).ToList());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create(int surveyId)
        {
            ViewBag.SurveyId = surveyId;
            ViewBag.QuestionTypes = new SelectList(new[]
            {
                new { Value = "SingleChoice", Text = "Jednokrotny wybór" },
                new { Value = "MultipleChoice", Text = "Wielokrotny wybór" }
            }, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("QuestionText,QuestionType,Order,IsRequired,SurveyId")] Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage), new { id = question.SurveyId });
            }

            ViewBag.SurveyId = question.SurveyId;
            ViewBag.QuestionTypes = new SelectList(new[]
            {
                new { Value = "SingleChoice", Text = "Jednokrotny wybór" },
                new { Value = "MultipleChoice", Text = "Wielokrotny wybór" }
            }, "Value", "Text");

            return View(question);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.QuestionTypes = new SelectList(new[]
            {
                new { Value = "SingleChoice", Text = "Jednokrotny wybór" },
                new { Value = "MultipleChoice", Text = "Wielokrotny wybór" }
            }, "Value", "Text", question.QuestionType);

            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionText,QuestionType,Order,IsRequired,SurveyId")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage), new { id = question.SurveyId });
            }

            ViewBag.QuestionTypes = new SelectList(new[]
            {
                new { Value = "SingleChoice", Text = "Jednokrotny wybór" },
                new { Value = "MultipleChoice", Text = "Wielokrotny wybór" }
            }, "Value", "Text", question.QuestionType);

            return View(question);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Survey)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            var surveyId = question.SurveyId;

            if (question.Answers != null && question.Answers.Any())
            {
                var answerIds = question.Answers.Select(a => a.Id).ToList();
                var responseDetails = await _context.ResponseDetails
                    .Where(rd => rd.AnswerId.HasValue && answerIds.Contains(rd.AnswerId.Value))
                    .ToListAsync();

                if (responseDetails.Any())
                {
                    _context.ResponseDetails.RemoveRange(responseDetails);
                }

                _context.Answers.RemoveRange(question.Answers);
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage), new { id = surveyId });
        }




        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
