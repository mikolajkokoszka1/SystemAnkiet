using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ResponseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResponseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Fill(int? id)
        {
            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).Include(s => s.Responses).FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            if (survey.Questions == null || !survey.Questions.Any())
            {
                TempData["Error"] = "Ta ankieta nie zawiera jeszcze żadnych pytań!";
                return RedirectToAction("Index", "Survey");
            }

            var questionWithoutEnoughAnswers = survey.Questions.FirstOrDefault(q => q.Answers == null || q.Answers.Count < 2);

            if (questionWithoutEnoughAnswers != null)
            {
                TempData["Error"] = $"Pytanie '{questionWithoutEnoughAnswers.QuestionText}' musi mieć przynajmniej 2 odpowiedzi!";
                return RedirectToAction("Index", "Survey");
            }

            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var hasResponded = survey.Responses?.Any(r => r.UserId == userId) ?? false;

            if (hasResponded)
            {
                return Forbid();
            }

            return View(survey);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Fill(int surveyId, IFormCollection form)
        {
            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
            {
                return NotFound();
            }

            var response = new Response
            {
                SurveyId = surveyId,
                SubmittedDate = DateTime.Now,
                UserId = User.Identity != null && User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null,
                IsAnonymous = User.Identity == null || !User.Identity.IsAuthenticated
            };

            _context.Responses.Add(response);
            await _context.SaveChangesAsync();

            if (survey.Questions != null)
            {
                foreach (var question in survey.Questions)
                {
                    var answerKey = $"answers[{question.Id}]";

                    if (question.QuestionType == "Text")
                    {
                        if (form.ContainsKey(answerKey) && !string.IsNullOrWhiteSpace(form[answerKey]))
                        {
                            _context.ResponseDetails.Add(new ResponseDetail
                            {
                                ResponseId = response.Id,
                                QuestionId = question.Id,
                                TextAnswer = form[answerKey].ToString()
                            });
                        }
                    }
                    else if (question.QuestionType == "SingleChoice")
                    {
                        if (form.ContainsKey(answerKey) && int.TryParse(form[answerKey], out int answerId))
                        {
                            _context.ResponseDetails.Add(new ResponseDetail
                            {
                                ResponseId = response.Id,
                                QuestionId = question.Id,
                                AnswerId = answerId
                            });
                        }
                    }
                    else if (question.QuestionType == "MultipleChoice")
                    {
                        var multiKey = $"multiAnswers_{question.Id}";

                        if (form.ContainsKey(multiKey))
                        {
                            var selectedAnswers = form[multiKey].ToString().Split(',');

                            foreach (var answerIdStr in selectedAnswers)
                            {
                                if (int.TryParse(answerIdStr, out int answerId))
                                {
                                    _context.ResponseDetails.Add(new ResponseDetail
                                    {
                                        ResponseId = response.Id,
                                        QuestionId = question.Id,
                                        AnswerId = answerId
                                    });
                                }
                            }
                        }
                    }

                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Dziękujemy za wypełnienie ankiety!";
            return RedirectToAction("ThankYou", new { id = response.Id });
        }

        public IActionResult ThankYou(int id)
        {
            ViewBag.ResponseId = id;
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Results(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var survey = await _context.Surveys.Include(s => s.Questions).ThenInclude(q => q.Answers).Include(s => s.Responses).ThenInclude(r => r.ResponseDetails).FirstOrDefaultAsync(s => s.Id == id);

            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }
    }
}
