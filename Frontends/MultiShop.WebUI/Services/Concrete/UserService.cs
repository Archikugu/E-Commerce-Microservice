using MultiShop.WebUI.Models;
using MultiShop.WebUI.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using System.Text.Json;

namespace MultiShop.WebUI.Services.Concrete;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UserDetailViewModel> GetUserDetails()
    {
        // Cookie'den token'ı al
        var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
        
        // Claims'den kullanıcı bilgilerini al
        var user = _httpContextAccessor.HttpContext.User;
        
        var id = user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
        var userName = user.FindFirst("preferred_username")?.Value ?? user.FindFirst("name")?.Value ?? user.FindFirst(ClaimTypes.Name)?.Value ?? "";
        var email = user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value ?? "";
        var firstName = user.FindFirst("given_name")?.Value ?? user.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
        var lastName = user.FindFirst("family_name")?.Value ?? user.FindFirst(ClaimTypes.Surname)?.Value ?? "";

        // Token varsa ve kritik alanlar eksikse userinfo'dan tamamla
        if (!string.IsNullOrWhiteSpace(accessToken) && (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email)))
        {
            try
            {
                var response = await _httpClient.GetAsync("/connect/userinfo");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (dict != null)
                    {
                        if (string.IsNullOrWhiteSpace(email) && dict.TryGetValue("email", out var em)) email = em?.ToString() ?? email;
                        if (string.IsNullOrWhiteSpace(firstName) && dict.TryGetValue("given_name", out var gn)) firstName = gn?.ToString() ?? firstName;
                        if (string.IsNullOrWhiteSpace(lastName) && dict.TryGetValue("family_name", out var fn)) lastName = fn?.ToString() ?? lastName;
                        // name'ten parçalama
                        if ((string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName)) && dict.TryGetValue("name", out var nm))
                        {
                            var full = nm?.ToString();
                            if (!string.IsNullOrWhiteSpace(full))
                            {
                                var parts = full.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 1)
                                {
                                    if (string.IsNullOrWhiteSpace(firstName)) firstName = parts[0];
                                }
                                else if (parts.Length > 1)
                                {
                                    if (string.IsNullOrWhiteSpace(firstName)) firstName = parts[0];
                                    if (string.IsNullOrWhiteSpace(lastName)) lastName = string.Join(' ', parts.Skip(1));
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // Fallback: given_name/family_name yoksa name'i "Ad Soyad" olarak parçala
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            var fullName = user.FindFirst("name")?.Value;
            if (!string.IsNullOrWhiteSpace(fullName))
            {
                var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    if (string.IsNullOrWhiteSpace(firstName)) firstName = parts[0];
                }
                else if (parts.Length > 1)
                {
                    if (string.IsNullOrWhiteSpace(firstName)) firstName = parts[0];
                    if (string.IsNullOrWhiteSpace(lastName)) lastName = string.Join(' ', parts.Skip(1));
                }
            }
        }

        return new UserDetailViewModel
        {
            Id = id,
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
    }
}
