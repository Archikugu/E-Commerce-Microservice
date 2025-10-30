using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ActivityController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var root = Directory.GetCurrentDirectory();
        var logFile = Path.Combine(root, "Frontends", "MultiShop.WebUI", "wwwroot", "logs", "admin-actions.log");
        var lines = System.IO.File.Exists(logFile) ? System.IO.File.ReadAllLines(logFile) : Array.Empty<string>();
        var last = lines.Reverse().Take(200).ToList();
        ViewBag.LogLines = last;
        return View();
    }
}


