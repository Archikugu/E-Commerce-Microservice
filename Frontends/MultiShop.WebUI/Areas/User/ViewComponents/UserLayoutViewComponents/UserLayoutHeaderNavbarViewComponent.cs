using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MultiShop.WebUI.Areas.User.ViewComponents.UserLayoutViewComponents;

public class UserLayoutHeaderNavbarViewComponent : ViewComponent
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserLayoutHeaderNavbarViewComponent(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public IViewComponentResult Invoke()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var firstName = user?.FindFirst("given_name")?.Value ?? user?.FindFirst(ClaimTypes.GivenName)?.Value;
        var lastName = user?.FindFirst("family_name")?.Value ?? user?.FindFirst(ClaimTypes.Surname)?.Value;
        var fullName = ($"{firstName} {lastName}").Trim();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            fullName = user?.FindFirst("name")?.Value
                ?? user?.FindFirst(ClaimTypes.Name)?.Value
                ?? user?.Identity?.Name
                ?? "User";
        }

        ViewBag.FullName = fullName;
        return View();
    }
}


