using MultiShop.WebUI.Dtos.IdentityDtos.UserDtos;
using MultiShop.WebUI.Dtos.IdentityDtos.Security;

namespace MultiShop.WebUI.Services.UserIdentityServices;

public interface IUserIdentityService
{
    Task<List<ResultUserDto>> GetAllUsersAsync();
    Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
}


