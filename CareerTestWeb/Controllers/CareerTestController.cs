using CareerTestWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
                    Answers = g.Select(q => new AnswerViewModel
                    {
                        IDAns = q.IDAns,
                        NameAns = q.NameAns
                    })
                    .OrderBy(a => a.IDAns)
                    .ToList()
                })
                .OrderBy(q => q.STT)
                .ToListAsync();

            return View(questions);
        }

        public async Task<IActionResult> QuickTest()
        {
            var rawData = await _context.CauHois
                .FromSqlRaw(@"
                    WITH RandomQuestions AS (
                        SELECT TOP 5 IDQues, NameQues, GroupID, STT
                        FROM (SELECT DISTINCT IDQues, NameQues, GroupID, STT FROM CauHoi WHERE GroupID = 0) t
                        ORDER BY NEWID()
                        UNION ALL
                        SELECT TOP 5 IDQues, NameQues, GroupID, STT
                        FROM (SELECT DISTINCT IDQues, NameQues, GroupID, STT FROM CauHoi WHERE GroupID = 1) t
                        ORDER BY NEWID()
                        UNION ALL
                        SELECT TOP 5 IDQues, NameQues, GroupID, STT
                        FROM (SELECT DISTINCT IDQues, NameQues, GroupID, STT FROM CauHoi WHERE GroupID = 2) t
                        ORDER BY NEWID()
                        UNION ALL
                        SELECT TOP 5 IDQues, NameQues, GroupID, STT
                        FROM (SELECT DISTINCT IDQues, NameQues, GroupID, STT FROM CauHoi WHERE GroupID = 3) t
                        ORDER BY NEWID()
                    )
                    SELECT rq.IDQues, rq.NameQues, rq.GroupID, rq.STT,
                           ch.IDAns, ch.NameAns,
                           0 AS QuestionType,
                           0 AS AnswerType
                    FROM RandomQuestions rq
                    JOIN CauHoi ch ON rq.IDQues = ch.IDQues
                    ORDER BY rq.GroupID, rq.IDQues, ch.IDAns
                ")
                .ToListAsync();

            var finalQuestions = rawData
                .GroupBy(q => new { q.IDQues, q.NameQues, q.STT })
                .Select(g => new QuestionViewModel
                {
                    STT = g.Key.STT,
                    IDQues = g.Key.IDQues,
                    NameQues = g.Key.NameQues,
                    Answers = g.Select(q => new AnswerViewModel
                    {
                        IDAns = q.IDAns,
                        NameAns = q.NameAns
                    })
                    .OrderBy(a => a.IDAns)
                    .ToList()
                })
                .OrderBy(q => q.STT)
                .ToList();

            return View("Test", finalQuestions);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitTest([FromBody] TestSubmission submission)
        {
            if (submission == null || submission.Answers == null || submission.Answers.Count == 0)
            {
                return BadRequest("No answers provided.");
            }

            var userId = submission.UserId;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                user = new Users
                {
                    UserID = userId,
                    Ten = "Người dùng mới",
                    NgayTest = DateTime.Now
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var oldAnswers = await _context.TraLois.Where(t => t.UserID == userId).ToListAsync();
            if (oldAnswers.Any())
            {
                _context.TraLois.RemoveRange(oldAnswers);
                await _context.SaveChangesAsync();
            }

            var traLois = submission.Answers
                .Select(a => new TraLoi { UserID = userId, IDQues = a.QuestionId, IDAns = a.AnswerId })
                .ToList();
            _context.TraLois.AddRange(traLois);
            await _context.SaveChangesAsync();

            var query = from tl in _context.TraLois
                        join ch in _context.CauHois on new { tl.IDQues, tl.IDAns } equals new { ch.IDQues, ch.IDAns }
                        join dac in _context.DapAnChuans on ch.IDQues equals dac.IDQues
                        where tl.UserID == userId
                        select new
                        {
                            ch.GroupID,
                            IsMatch = tl.IDAns == dac.IDAns
                        };

            var results = await query.ToListAsync();

            var groupStats = results
                .GroupBy(r => r.GroupID)
                .Select(g => new
                {
                    GroupID = g.Key,
                    Total = g.Count(),
                    Match = g.Count(x => x.IsMatch)
                })
                .ToDictionary(x => x.GroupID, x => x);

            float scoreEI = groupStats.ContainsKey(0) ? (float)groupStats[0].Match / groupStats[0].Total : 0;
            float scoreSN = groupStats.ContainsKey(1) ? (float)groupStats[1].Match / groupStats[1].Total : 0;
            float scoreTF = groupStats.ContainsKey(2) ? (float)groupStats[2].Match / groupStats[2].Total : 0;
            float scoreJP = groupStats.ContainsKey(3) ? (float)groupStats[3].Match / groupStats[3].Total : 0;

            string mbti = (scoreEI >= 0.5 ? "E" : "I") +
                          (scoreSN >= 0.5 ? "S" : "N") +
                          (scoreTF >= 0.5 ? "T" : "F") +
                          (scoreJP >= 0.5 ? "J" : "P");

            var careers = await _context.NgheNghieps
                .Where(n => n.MBTI == mbti)
                .Select(n => new CareerOption
                {
                    TenNghe = n.TenNghe,
                    ThuNhapTB = n.ThuNhapTB,
                    CoHoiViecLam = n.CoHoiViecLam,
                    MucDoPhuHop = n.MucDoPhuHop,
                    CoHoiThangTien = n.CoHoiThangTien,
                    DoOnDinh = n.DoOnDinh
                })
                .OrderByDescending(c => c.MucDoPhuHop)
                .ToListAsync();

            var result = new
            {
                PersonalityType = mbti,
                ScoreEI = scoreEI * 100,
                ScoreSN = scoreSN * 100,
                ScoreTF = scoreTF * 100,
                ScoreJP = scoreJP * 100,
                Careers = careers
            };

            return Json(result);
        }

        public async Task<IActionResult> Result(string type)
        {
            if (string.IsNullOrEmpty(type))
                type = "ESFJ";

            var personality = await _context.TinhCachMBTIs.FirstOrDefaultAsync(x => x.MBTI == type);
            var careers = await _context.NgheNghieps
                .Where(n => n.MBTI == type)
                .Select(n => new CareerOption
                {
                    TenNghe = n.TenNghe,
                    ThuNhapTB = n.ThuNhapTB,
                    CoHoiViecLam = n.CoHoiViecLam,
                    MucDoPhuHop = n.MucDoPhuHop,
                    CoHoiThangTien = n.CoHoiThangTien,
                    DoOnDinh = n.DoOnDinh
                })
                .OrderByDescending(n => n.MucDoPhuHop)
                .ToListAsync();

            var result = new ResultViewModel
            {
                PersonalityType = type,
                TenNhom = personality?.TenNhom,
                MoTa = personality?.MoTa,
                UuDiem = personality?.UuDiem,
                NhuocDiem = personality?.NhuocDiem,
                TopCareer = careers.FirstOrDefault(),
                OtherCareers = careers.Skip(1).ToList(),
                Careers = careers
            };

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> FilterCareers([FromBody] FilterRequest request)
        {
            Console.WriteLine("[DEBUG] FilterCareers - Received request: " + (request != null ? JsonSerializer.Serialize(request) : "null"));

            if (request == null || string.IsNullOrEmpty(request.PersonalityType) || request.SelectedCriteria == null || request.SelectedCriteria.Count != 5)
                return BadRequest("Invalid request. PersonalityType and exactly 5 SelectedCriteria are required.");

            if (!await _context.TinhCachMBTIs.AnyAsync(t => t.MBTI == request.PersonalityType))
                return BadRequest("Invalid MBTI type: " + request.PersonalityType);

            var careers = await _context.NgheNghieps
                .Where(n => n.MBTI == request.PersonalityType)
                .Select(n => new CareerOption
                {
                    TenNghe = n.TenNghe,
                    ThuNhapTB = n.ThuNhapTB,
                    CoHoiViecLam = n.CoHoiViecLam,
                    MucDoPhuHop = n.MucDoPhuHop,
                    CoHoiThangTien = n.CoHoiThangTien,
                    DoOnDinh = n.DoOnDinh
                })
                .ToListAsync();

            if (!careers.Any())
                return Json(new { TopCareers = new List<CareerOption>(), AllCareers = new List<CareerOption>() });

            var selectedIndexes = request.SelectedCriteria
                .Select((v, i) => new { v, i })
                .Where(x => x.v)
                .Select(x => x.i)
                .ToList();

            List<(CareerOption Career, double Score)> scoredCareers;

            // --- 🟢 Nếu chỉ chọn 1 tiêu chí: lọc cứng ---
            if (selectedIndexes.Count == 1)
            {
                int idx = selectedIndexes[0];
                scoredCareers = careers.Select(c =>
                {
                    double s = idx switch
                    {
                        0 => c.ThuNhapTB,
                        1 => c.CoHoiViecLam,
                        2 => c.MucDoPhuHop,
                        3 => c.CoHoiThangTien,
                        4 => c.DoOnDinh,
                        _ => 0
                    };
                    return (c, s);
                }).ToList();
                Console.WriteLine("[DEBUG] FilterCareers - Using single-criteria mode on index " + idx);
            }
            else
            {
                // --- 🟢 AHP như cũ ---
                if (!selectedIndexes.Any())
                    selectedIndexes = new List<int> { 0, 1, 2, 3, 4 };

                var pairWiseMatrix = new double[5, 5];
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (i == j) pairWiseMatrix[i, j] = 1;
                        else if (request.SelectedCriteria[i] && !request.SelectedCriteria[j]) pairWiseMatrix[i, j] = 3;
                        else if (!request.SelectedCriteria[i] && request.SelectedCriteria[j]) pairWiseMatrix[i, j] = 1.0 / 3;
                        else pairWiseMatrix[i, j] = 1;
                        pairWiseMatrix[j, i] = 1.0 / pairWiseMatrix[i, j];
                    }
                }

                var columnSums = new double[5];
                for (int j = 0; j < 5; j++)
                    for (int i = 0; i < 5; i++)
                        columnSums[j] += pairWiseMatrix[i, j];

                var weights = new double[5];
                for (int i = 0; i < 5; i++)
                {
                    double rowSum = 0;
                    for (int j = 0; j < 5; j++)
                        rowSum += pairWiseMatrix[i, j] / columnSums[j];
                    weights[i] = rowSum / 5;
                }

                scoredCareers = careers.Select(c => (
                    c,
                    (c.ThuNhapTB * weights[0]) +
                    (c.CoHoiViecLam * weights[1]) +
                    (c.MucDoPhuHop * weights[2]) +
                    (c.CoHoiThangTien * weights[3]) +
                    (c.DoOnDinh * weights[4])
                )).ToList();
                Console.WriteLine("[DEBUG] FilterCareers - Using AHP mode. Weights=" + string.Join(",", weights));
            }

            var maxScore = scoredCareers.Max(c => c.Score);
            var topCareers = scoredCareers
                .Where(c => c.Score == maxScore)
                .Select(c => c.Career)
                .ToList();

            return Json(new { TopCareers = topCareers, AllCareers = careers });
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

    public class QuestionViewModel
    {
        public int STT { get; set; }
        public int IDQues { get; set; }
        public string? NameQues { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new();
    }

    public class AnswerViewModel
    {
        public int IDAns { get; set; }
        public string? NameAns { get; set; }
    }

    public class ResultViewModel
    {
        public string PersonalityType { get; set; } = string.Empty;
        public string? TenNhom { get; set; }
        public string? MoTa { get; set; }
        public string? UuDiem { get; set; }
        public string? NhuocDiem { get; set; }
        public CareerOption? TopCareer { get; set; }
        public List<CareerOption> OtherCareers { get; set; } = new();
        public List<CareerOption> Careers { get; set; } = new();
    }

    public class CareerOption
    {
        public string TenNghe { get; set; }
        public int ThuNhapTB { get; set; }
        public int CoHoiViecLam { get; set; }
        public int MucDoPhuHop { get; set; }
        public int CoHoiThangTien { get; set; }
        public int DoOnDinh { get; set; }
    }

    public class FilterRequest
    {
        public string PersonalityType { get; set; }
        public List<bool> SelectedCriteria { get; set; } // [ThuNhapTB, CoHoiViecLam, MucDoPhuHop, CoHoiThangTien, DoOnDinh]
    }
}