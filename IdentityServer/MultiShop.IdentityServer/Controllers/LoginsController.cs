using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.IdentityServer.Dtos;
using MultiShop.IdentityServer.Models;
using MultiShop.IdentityServer.Tools;
using Microsoft.EntityFrameworkCore;

namespace MultiShop.IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginsController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginsController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> UserLogin(UserLoginDto userLoginDto)
    {
        // Kullanıcı adı veya email ile tek sorguda arama
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == userLoginDto.LoginIdentifier || u.Email == userLoginDto.LoginIdentifier);

        if (user == null)
        {
            return BadRequest("User not found. Please check your username or email address.");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName, // Login with username
            userLoginDto.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            GetCheckAppUserViewModel model = new GetCheckAppUserViewModel();
            model.UserName = user.UserName;
            model.Id = user.Id;
            model.Email = user.Email;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            var tokenResponse = JwtTokenGenerator.GenerateToken(model);
            return Ok(tokenResponse.Token); // Sadece token string'ini döndür
        }
        else
        {
            return BadRequest("Invalid login attempt. Please check your password.");
        }
    }
}
