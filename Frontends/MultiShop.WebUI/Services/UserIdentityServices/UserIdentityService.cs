using MultiShop.WebUI.Dtos.IdentityDtos.UserDtos;
using MultiShop.WebUI.Dtos.IdentityDtos.Security;

namespace MultiShop.WebUI.Services.UserIdentityServices;

public class UserIdentityService : IUserIdentityService
{
    private readonly HttpClient _httpClient;

    public UserIdentityService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResultUserDto>> GetAllUsersAsync()
    {
        var resp = await _httpClient.GetAsync("api/Users/GetAllUsers");
        if (!resp.IsSuccessStatusCode)
        {
            return new List<ResultUserDto>();
        }
        return await resp.Content.ReadFromJsonAsync<List<ResultUserDto>>() ?? new List<ResultUserDto>();
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
    {
        var resp = await _httpClient.PostAsJsonAsync("api/Users/ChangePassword", dto);
        if (resp.IsSuccessStatusCode)
        {
            return true;
        }
        var content = await resp.Content.ReadAsStringAsync();
        throw new Exception(string.IsNullOrWhiteSpace(content) ? "Failed to change password" : content);
    }
}


