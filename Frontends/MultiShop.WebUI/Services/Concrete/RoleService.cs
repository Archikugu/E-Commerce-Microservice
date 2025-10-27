using System.Net.Http.Json;
using MultiShop.WebUI.Dtos.IdentityDtos.RoleDtos;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Services.Concrete;

public class RoleService : IRoleService
{
    private readonly HttpClient _httpClient;

    public RoleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResultRoleDto>> GetAllAsync()
    {
        var resp = await _httpClient.GetAsync("api/Roles");
        if (!resp.IsSuccessStatusCode) return new List<ResultRoleDto>();
        return await resp.Content.ReadFromJsonAsync<List<ResultRoleDto>>() ?? new List<ResultRoleDto>();
    }

    public async Task<ResultRoleDto?> GetByIdAsync(string id)
    {
        var resp = await _httpClient.GetAsync($"api/Roles/{id}");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<ResultRoleDto?>();
    }

    public async Task<bool> CreateAsync(CreateRoleDto dto)
    {
        var resp = await _httpClient.PostAsJsonAsync("api/Roles", new { name = dto.Name });
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateAsync(UpdateRoleDto dto)
    {
        var resp = await _httpClient.PutAsJsonAsync("api/Roles", new { id = dto.Id, name = dto.Name });
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var resp = await _httpClient.DeleteAsync($"api/Roles/{id}");
        return resp.IsSuccessStatusCode;
    }
}


