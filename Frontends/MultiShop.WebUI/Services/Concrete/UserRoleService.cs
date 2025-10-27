using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Services.Concrete;

public class UserRoleService : IUserRoleService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public UserRoleService(HttpClient httpClient, IMemoryCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
    }

    public async Task<List<AssignRoleItemDto>> GetAllRolesAsync()
    {
        if (_cache.TryGetValue("all_roles", out List<AssignRoleItemDto> cached) && cached != null)
            return cached;

        var resp = await _httpClient.GetAsync("api/Roles");
        if (!resp.IsSuccessStatusCode) return new List<AssignRoleItemDto>();
        var roles = await resp.Content.ReadFromJsonAsync<List<RoleSimpleDto>>() ?? new List<RoleSimpleDto>();
        var mapped = roles.Select(r => new AssignRoleItemDto { RoleId = r.Id, RoleName = r.Name }).ToList();
        _cache.Set("all_roles", mapped, TimeSpan.FromMinutes(5));
        return mapped;
    }

    public async Task<List<string>> GetUserRolesAsync(string userId)
    {
        var resp = await _httpClient.GetAsync($"api/UserRoles/{userId}/roles");
        if (!resp.IsSuccessStatusCode) return new List<string>();
        var roles = await resp.Content.ReadFromJsonAsync<List<string>>();
        return roles ?? new List<string>();
    }

    public async Task<bool> ToggleUserRoleAsync(string userId, string roleName, bool isInRole)
    {
        var resp = await _httpClient.PostAsJsonAsync("api/UserRoles/toggle", new { userId, roleName, isInRole });
        return resp.IsSuccessStatusCode;
    }

    private class RoleSimpleDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}


