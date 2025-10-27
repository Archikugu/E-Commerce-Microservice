using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiShop.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;

namespace MultiShop.IdentityServer.Controllers;

[Authorize(LocalApi.PolicyName)]
[Route("api/[controller]")]
[ApiController]
public class UserRolesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRolesController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public record AssignRoleRequest(string? UserId, string? RoleName);
    public record ToggleRoleRequest(string? UserId, string? RoleName, bool IsInRole);

    [AllowAnonymous]
    [HttpGet("{userId}/roles")]
    public async Task<IActionResult> GetUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound("User not found");
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(roles);
    }

    [AllowAnonymous]
    [HttpGet("available-roles")]
    public IActionResult GetAllRoles()
    {
        var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
        return Ok(roles);
    }

    [AllowAnonymous]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.UserId) || string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest("UserId and RoleName are required");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
            return NotFound("Role not found");

        // Idempotent: if already in role, return OK
        if (await _userManager.IsInRoleAsync(user, request.RoleName))
            return Ok("Already assigned");

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        return Ok("Role assigned");
    }

    [AllowAnonymous]
    [HttpPost("remove")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.UserId) || string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest("UserId and RoleName are required");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
            return NotFound("Role not found");

        // Idempotent: if not in role, return OK
        if (!await _userManager.IsInRoleAsync(user, request.RoleName))
            return Ok("Already removed");

        var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
        if (!result.Succeeded)
            return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        return Ok("Role removed");
    }

    [AllowAnonymous]
    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleRole([FromBody] ToggleRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.UserId) || string.IsNullOrWhiteSpace(request.RoleName))
            return BadRequest("UserId and RoleName are required");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null) return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync(request.RoleName))
            return NotFound("Role not found");

        var isInRole = await _userManager.IsInRoleAsync(user, request.RoleName);
        if (request.IsInRole && !isInRole)
        {
            var addResult = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!addResult.Succeeded)
                return BadRequest(string.Join("; ", addResult.Errors.Select(e => e.Description)));
        }
        else if (!request.IsInRole && isInRole)
        {
            var removeResult = await _userManager.RemoveFromRoleAsync(user, request.RoleName);
            if (!removeResult.Succeeded)
                return BadRequest(string.Join("; ", removeResult.Errors.Select(e => e.Description)));
        }

        return Ok("Role toggled");
    }
}


