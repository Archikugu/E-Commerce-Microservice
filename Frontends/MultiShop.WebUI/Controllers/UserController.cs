using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Controllers;

public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var values = await _userService.GetUserDetails();
        return View(values);
    }
}
