using MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;

namespace MultiShop.WebUI.Services.Abstract;

public interface IRoleService
{
    Task<List<ResultRoleDto>> GetAllAsync();
    Task<ResultRoleDto?> GetByIdAsync(string id);
    Task<bool> CreateAsync(CreateRoleDto dto);
    Task<bool> UpdateAsync(UpdateRoleDto dto);
    Task<bool> DeleteAsync(string id);
}


