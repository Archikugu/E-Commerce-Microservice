using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;

namespace MultiShop.IdentityServer.Controllers;

[Authorize(LocalApi.PolicyName)]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("GetUserDetails")]
    public async Task<IActionResult> GetUserDetails()
    {
        var userClaim = User.Claims.FirstOrDefault(u=>u.Type == JwtRegisteredClaimNames.Sub);
        var user = await _userManager.FindByIdAsync(userClaim.Value);
        return Ok(new
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            UserName= user.UserName
        });
    }

    [HttpGet("GetAllUsers")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users
            .Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.NormalizedEmail,
                u.EmailConfirmed,
                u.UserName,
                u.NormalizedUserName,
                u.PhoneNumber,
                u.PhoneNumberConfirmed,
                u.TwoFactorEnabled,
                u.LockoutEnd,
                u.LockoutEnabled,
                u.AccessFailedCount,
                u.SecurityStamp,
                u.ConcurrencyStamp
            })
            .ToListAsync();

        return Ok(users);
    }
}
