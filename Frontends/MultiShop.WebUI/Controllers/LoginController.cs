using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.IdentityDtos.LoginDtos;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Controllers;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IIdentityService _identityService;

    public LoginController(IHttpClientFactory httpClientFactory, IIdentityService identityService)
    {
        _httpClientFactory = httpClientFactory;
        _identityService = identityService;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Index(CreateLoginDto createLoginDto)
    {
        try
        {
            // CreateLoginDto'dan SignInDto'ya dönüştür
            var signInDto = new SignInDto
            {
                UserName = createLoginDto.LoginIdentifier, // Email veya Username
                Password = createLoginDto.Password
            };
            
            var result = await _identityService.SignIn(signInDto);
            if (result)
            {
                return RedirectToAction("Index", "User");
            }
            else
            {
                TempData["ErrorMessage"] = "Login failed! Invalid username, email or password.";
                return View(createLoginDto);
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Login error: {ex.Message}";
            return View(createLoginDto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        // Tüm auth çerezlerini temizle ve ana sayfaya dön
        await HttpContext.SignOutAsync();
        return Redirect("/");
    }
}
