using MultiShop.WebUI.Dtos.IdentityDtos.UserDtos;

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
}


