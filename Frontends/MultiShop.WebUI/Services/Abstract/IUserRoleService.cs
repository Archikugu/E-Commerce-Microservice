using MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;

namespace MultiShop.WebUI.Services.Abstract;

public interface IUserRoleService
{
    Task<List<AssignRoleItemDto>> GetAllRolesAsync();
    Task<List<string>> GetUserRolesAsync(string userId);
    Task<bool> ToggleUserRoleAsync(string userId, string roleName, bool isInRole);
}


