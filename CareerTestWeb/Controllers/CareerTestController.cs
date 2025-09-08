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
                .OrderBy(q => q.STT)
                .ToListAsync();

            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTest([FromBody] TestSubmission submission)
        {
            // Xử lý kết quả bài test và tính điểm MBTI
            // Lưu câu trả lời vào database
            // Tính toán kết quả MBTI

            // Giả lập kết quả cho demo
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
            // Hiển thị kết quả chi tiết
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
}