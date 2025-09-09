using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CareerTestWeb.Data;
using CareerTestWeb.Models;

namespace CareerTestWeb.Controllers
{
    public class CareerTestController : Controller
    {
        private readonly MBTIDbContext _context;

        public CareerTestController(MBTIDbContext context)
        {
            _context = context;
        }

        // GET: CareerTest/Test
        public async Task<IActionResult> Test()
        {
            // Lấy tất cả câu hỏi từ database
            var allData = await _context.CauHoi.ToListAsync();

            // Nhóm các câu hỏi theo IDQues để tránh trùng lặp
            var groupedQuestions = allData
                .GroupBy(q => q.IDQues)
                .Select(g => new CauHoi
                {
                    IDQues = g.Key,
                    NameQues = g.First().NameQues,
                    QuestionType = g.First().QuestionType,
                    // Lưu tất cả câu trả lời cho câu hỏi này
                    Answers = g.Select(a => new Answer
                    {
                        IDAns = a.IDAns,
                        NameAns = a.NameAns,
                        AnswerType = a.AnswerType,
                        GroupID = a.GroupID
                    }).ToList()
                })
                .ToList();

            return View(groupedQuestions);
        }

        // Model classes cho submission data
        public class TestSubmission
        {
            public List<AnswerSubmission> Answers { get; set; }
        }

        public class AnswerSubmission
        {
            public int QuestionId { get; set; }
            public int AnswerId { get; set; }
            public int AnswerType { get; set; }
            public int GroupId { get; set; }
        }

        // POST: CareerTest/SubmitTest
        [HttpPost]
        public async Task<IActionResult> SubmitTest([FromBody] TestSubmission submission)
        {
            try
            {
                // Lấy UserID từ session hoặc authentication
                var userId = GetCurrentUserId();

                // Xóa các câu trả lời cũ của user nếu có
                var oldAnswers = _context.TraLoi.Where(t => t.UserId == userId);
                _context.TraLoi.RemoveRange(oldAnswers);

                // Lưu các câu trả lời mới
                foreach (var answer in submission.Answers)
                {
                    var traLoi = new TraLoi
                    {
                        UserId = userId,
                        Idques = answer.QuestionId,
                        Idans = answer.AnswerId
                    };
                    _context.TraLoi.Add(traLoi);
                }

                await _context.SaveChangesAsync();

                // Tính toán kết quả MBTI dựa trên GroupID và AnswerType
                var personalityType = CalculateMBTI(submission.Answers);

                return Ok(new
                {
                    success = true,
                    personalityType = personalityType
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private int GetCurrentUserId()
        {
            // TODO: Thay thế bằng logic lấy UserID thực tế
            // Ví dụ từ authentication:
            // var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            // if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            //     return userId;

            return 1; // Tạm thời return 1 cho testing
        }

        private string CalculateMBTI(List<AnswerSubmission> answers)
        {
            // Logic tính MBTI dựa trên GroupID
            var scores = new Dictionary<int, int> { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } };

            foreach (var answer in answers)
            {
                // Mỗi answer có GroupID từ 0-3 tương ứng với 4 cặp đối lập MBTI
                if (scores.ContainsKey(answer.GroupId))
                {
                    scores[answer.GroupId] += answer.AnswerType == 1 ? 1 : -1;
                }
            }

            var result = "";
            result += scores[0] > 0 ? "E" : "I";  // Extraversion vs Introversion
            result += scores[1] > 0 ? "S" : "N";  // Sensing vs Intuition
            result += scores[2] > 0 ? "T" : "F";  // Thinking vs Feeling
            result += scores[3] > 0 ? "J" : "P";  // Judging vs Perceiving

            return result;
        }

        // GET: CareerTest/Result
        public IActionResult Result(string type)
        {
            // Hiển thị kết quả MBTI
            ViewData["PersonalityType"] = type;
            return View();
        }
    }

    // Model cho Answer (thêm trong cùng file hoặc tạo file riêng)
    public class Answer
    {
        public int IDAns { get; set; }
        public string NameAns { get; set; }
        public int AnswerType { get; set; }
        public int GroupID { get; set; }
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