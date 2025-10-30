using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.IdentityDtos.Security;
using MultiShop.WebUI.Services.UserIdentityServices;
using System.Text.Json;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class AccountController : Controller
{
    private readonly IUserIdentityService _userIdentityService;

    public AccountController(IUserIdentityService userIdentityService)
    {
        _userIdentityService = userIdentityService;
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View(new ChangePasswordDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.OldPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            ModelState.AddModelError(string.Empty, "Please fill all fields.");
            return View(dto);
        }
        if (!string.Equals(dto.NewPassword, dto.ConfirmNewPassword))
        {
            ModelState.AddModelError(string.Empty, "New password and confirmation do not match.");
            return View(dto);
        }
        try
        {
            var ok = await _userIdentityService.ChangePasswordAsync(dto);
            if (ok)
            {
                await HttpContext.SignOutAsync();
                return RedirectToAction("Index", "Login", new { area = "", pc = 1 });
            }
        }
        catch (Exception ex)
        {
            // Try to parse API error and map to friendly messages
            var added = false;
            try
            {
                using var doc = JsonDocument.Parse(ex.Message);
                if (doc.RootElement.TryGetProperty("errors", out var errorsEl) && errorsEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var el in errorsEl.EnumerateArray())
                    {
                        var raw = el.GetString() ?? string.Empty;
                        var friendly = MapPasswordError(raw);
                        if (!string.IsNullOrWhiteSpace(friendly))
                        {
                            ModelState.AddModelError(string.Empty, friendly);
                            added = true;
                        }
                    }
                }
            }
            catch { }

            if (!added)
            {
                ModelState.AddModelError(string.Empty, "Password change failed. Please verify the fields and try again.");
            }
        }
        return View(dto);
    }

    private static string MapPasswordError(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        raw = raw.Trim();
        if (raw.Contains("at least 6 characters", StringComparison.OrdinalIgnoreCase))
            return "Password must be at least 6 characters.";
        if (raw.Contains("one non alphanumeric character", StringComparison.OrdinalIgnoreCase))
            return "Include at least one special character (e.g., !@#$).";
        if (raw.Contains("one digit", StringComparison.OrdinalIgnoreCase))
            return "Include at least one number.";
        if (raw.Contains("one uppercase", StringComparison.OrdinalIgnoreCase))
            return "Include at least one uppercase letter.";
        if (raw.Contains("one lowercase", StringComparison.OrdinalIgnoreCase))
            return "Include at least one lowercase letter.";
        return raw;
    }
}


