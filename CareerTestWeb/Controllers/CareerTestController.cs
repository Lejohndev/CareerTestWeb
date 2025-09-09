using CareerTestWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Controllers
{
    public class CareerTestController : Controller
    {
        private readonly MBTIDbContext _context;

        public CareerTestController(MBTIDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Test()
        {
            var questions = await _context.CauHois
                .GroupBy(q => new { q.IDQues, q.NameQues, q.STT })
                .Select(g => new QuestionViewModel
                {
                    STT = g.Key.STT,
                    IDQues = g.Key.IDQues,
                    NameQues = g.Key.NameQues,
                    Answers = g.Select(q => new AnswerViewModel { IDAns = q.IDAns, NameAns = q.NameAns })
                              .OrderBy(a => a.IDAns)
                              .ToList()
                })
                .OrderBy(q => q.STT)
                .ToListAsync();

            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTest([FromBody] TestSubmission submission)
        {
            var result = new
            {
                PersonalityType = "INTJ",
                ScoreEI = 75,
                ScoreSN = 60,
                ScoreTF = 80,
                ScoreJP = 65,
                Careers = new[] {
                    new { Name = "Kỹ sư phần mềm", Match = 92 },
                    new { Name = "Nhà khoa học dữ liệu", Match = 88 },
                    new { Name = "Quản lý dự án", Match = 85 }
                }
            };

            return Json(result);
        }

        public IActionResult Result(string type)
        {
            ViewBag.PersonalityType = type ?? "INTJ";
            return View();
        }
    }

    public class TestSubmission
    {
        public int UserId { get; set; }
        public List<QuestionAnswer> Answers { get; set; }
    }

    public class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }

    // View model cho câu hỏi
    public class QuestionViewModel
    {
        public int STT { get; set; }
        public int IDQues { get; set; }
        public string? NameQues { get; set; }
        public List<AnswerViewModel> Answers { get; set; } // Thay List<dynamic> bằng List<AnswerViewModel>
    }

    // View model cho đáp án
    public class AnswerViewModel
    {
        public int IDAns { get; set; }
        public string? NameAns { get; set; }
    }
}