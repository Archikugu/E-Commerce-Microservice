using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.IdentityServer.Dtos;
using MultiShop.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;

namespace MultiShop.IdentityServer.Controllers;

//[Authorize(LocalApi.PolicyName)]
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class RegistersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public RegistersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> UserRegister(UserRegisterDto userRegisterDto)
    {
        var user = new ApplicationUser()
        {
            UserName = userRegisterDto.UserName,
            Email = userRegisterDto.Email,
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
        };
        var result = await _userManager.CreateAsync(user, userRegisterDto.Password);

        if (result.Succeeded)
        {
            return Ok("User has been successfully registered!");
        }
        else
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(errors);
        }

    }

}
