using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;
using MultiShop.WebUI.Dtos.IdentityDtos.UserDtos;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.UserIdentityServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRoleController : Controller
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserIdentityService userIdentityService, IUserRoleService userRoleService)
        {
            _userIdentityService = userIdentityService;
            _userRoleService = userRoleService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userIdentityService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Assign(string id, string? fullName)
        {
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction("Index");

            var allRoles = await _userRoleService.GetAllRolesAsync();
            var userRoles = await _userRoleService.GetUserRolesAsync(id);
            var userRolesSet = new HashSet<string>(userRoles ?? new List<string>(), StringComparer.OrdinalIgnoreCase);

            var model = new AssignRoleDto
            {
                UserId = id,
                UserFullName = fullName
            };

            model.Roles = allRoles
                .Select(r => new AssignRoleItemDto
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,
                    IsAssigned = !string.IsNullOrWhiteSpace(r.RoleName) && userRolesSet.Contains(r.RoleName)
                })
                .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(AssignRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.UserId)) return RedirectToAction("Index");

            int total = 0;
            int success = 0;
            foreach (var item in dto.Roles ?? new List<AssignRoleItemDto>())
            {
                if (!string.IsNullOrWhiteSpace(item.RoleName))
                {
                    total++;
                    var ok = await _userRoleService.ToggleUserRoleAsync(dto.UserId!, item.RoleName!, item.IsAssigned);
                    if (ok) success++;
                }
            }

            if (total == success)
                TempData["SuccessMessage"] = "Roles updated successfully.";
            else if (success == 0)
                TempData["ErrorMessage"] = "Rol ataması başarısız. Lütfen tekrar deneyin.";
            else
                TempData["WarningMessage"] = $"Kısmen güncellendi. {success}/{total} rol güncellendi.";
            return RedirectToAction("Assign", new { id = dto.UserId, fullName = dto.UserFullName });
        }
    }
}


