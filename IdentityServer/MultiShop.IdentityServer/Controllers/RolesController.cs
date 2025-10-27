using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Duende.IdentityServer.IdentityServerConstants;

namespace MultiShop.IdentityServer.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleManager.Roles
            .Select(r => new { r.Id, r.Name, r.NormalizedName, r.ConcurrencyStamp })
            .ToListAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        return Ok(new { role.Id, role.Name, role.NormalizedName, role.ConcurrencyStamp });
    }

    public class CreateRoleRequest { public string? Name { get; set; } }
    public class UpdateRoleRequest { public string? Id { get; set; } public string? Name { get; set; } }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name)) return BadRequest("Name is required");
        var exists = await _roleManager.RoleExistsAsync(request.Name);
        if (exists) return Conflict("Role already exists");
        var result = await _roleManager.CreateAsync(new IdentityRole(request.Name));
        if (!result.Succeeded) return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));
        return Ok("Role created");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRoleRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Id) || string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Id and Name are required");
        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role == null) return NotFound();
        role.Name = request.Name;
        role.NormalizedName = request.Name.ToUpperInvariant();
        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded) return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));
        return Ok("Role updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();
        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded) return BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));
        return Ok("Role deleted");
    }
}


