using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AnswerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnswerController(ApplicationDbContext context)
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

            var question = await _context.Questions.Include(q => q.Answers).Include(q => q.Survey).FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.QuestionId = id;
            ViewBag.QuestionText = question.QuestionText;
            ViewBag.SurveyId = question.SurveyId;

            return View(question.Answers.OrderBy(a => a.Order).ToList());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return NotFound();
            }

            ViewBag.QuestionId = questionId;
            ViewBag.QuestionText = question.QuestionText;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("AnswerText,Order,QuestionId")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Manage), new { id = answer.QuestionId });
            }

            ViewBag.QuestionId = answer.QuestionId;
            return View(answer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers.Include(a => a.Question).FirstOrDefaultAsync(a => a.Id == id);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnswerText,Order,QuestionId")] Answer answer)
        {
            if (id != answer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(answer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerExists(answer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Manage), new { id = answer.QuestionId });
            }
            return View(answer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers.Include(a => a.Question).FirstOrDefaultAsync(m => m.Id == id);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var answer = await _context.Answers.FindAsync(id);

            if (answer == null)
            {
                return NotFound();
            }

            var questionId = answer.QuestionId;

            var responseDetails = await _context.ResponseDetails.Where(rd => rd.AnswerId == id).ToListAsync();

            if (responseDetails.Any())
            {
                _context.ResponseDetails.RemoveRange(responseDetails);
            }

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage), new { id = questionId });
        }



        private bool AnswerExists(int id)
        {
            return _context.Answers.Any(e => e.Id == id);
        }
    }
}
