using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;
using System.Threading.Tasks;

namespace MultiShop.IdentityServer.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class StatisticsController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public StatisticsController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("user-count")]
    public IActionResult GetUserCount()
    {
        // IQueryable count executes in DB when enumerated
        var count = _userManager.Users.LongCount();
        return Ok(count);
    }
}


