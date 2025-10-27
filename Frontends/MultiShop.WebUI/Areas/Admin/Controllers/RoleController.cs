using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllAsync();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateRoleDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Name))
            {
                ModelState.AddModelError(nameof(dto.Name), "Role name is required.");
                return View(dto);
            }
            var ok = await _roleService.CreateAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to create role.");
                return View(dto);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null) return RedirectToAction("Index");
            return View(new UpdateRoleDto { Id = role.Id, Name = role.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Id) || string.IsNullOrWhiteSpace(dto.Name))
            {
                ModelState.AddModelError(string.Empty, "Invalid input.");
                return View(dto);
            }
            var ok = await _roleService.UpdateAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "Failed to update role.");
                return View(dto);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                await _roleService.DeleteAsync(id);
            }
            return RedirectToAction("Index");
        }
    }
}


