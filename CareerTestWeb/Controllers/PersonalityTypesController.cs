using Microsoft.AspNetCore.Mvc;

public class PersonalityTypesController : Controller
{
    public IActionResult Index()
    {
        // Hiển thị danh sách tất cả các nhóm tính cách
        return View();
    }

    [Route("PersonalityTypes/{type}")]
    public IActionResult Detail(string type)
    {
        // Kiểm tra type có hợp lệ không
        var validTypes = new[] { "ENFJ", "ENFP", "ENTJ", "ENTP", "ESTJ", "ESFP", "ESTP", "INFJ", "INFP", "INTJ", "INTP", "ISFJ", "ISFP", "ISTJ", "ISTP" };

        if (!validTypes.Contains(type.ToUpper()))
        {
            return NotFound();
        }

        // Truyền type đến view
        ViewBag.PersonalityType = type.ToUpper();
        return View(type);
    }
}