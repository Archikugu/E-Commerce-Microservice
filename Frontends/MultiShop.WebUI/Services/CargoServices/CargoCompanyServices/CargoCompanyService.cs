using MultiShop.WebUI.Dtos.CargoDtos.CargoCompanyDtos;

namespace MultiShop.WebUI.Services.CargoServices.CargoCompanyServices;

public class CargoCompanyService : ICargoCompanyService
{
    private readonly HttpClient _httpClient;

    public CargoCompanyService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResultCargoCompanyDto>> GetAllAsync()
    {
        var resp = await _httpClient.GetAsync("cargocompanies");
        if (!resp.IsSuccessStatusCode)
        {
            return new List<ResultCargoCompanyDto>();
        }
        return await resp.Content.ReadFromJsonAsync<List<ResultCargoCompanyDto>>() ?? new List<ResultCargoCompanyDto>();
    }

    public async Task<ResultCargoCompanyDto?> GetByIdAsync(int id)
    {
        var resp = await _httpClient.GetAsync($"cargocompanies/{id}");
        if (!resp.IsSuccessStatusCode)
        {
            return null;
        }
        return await resp.Content.ReadFromJsonAsync<ResultCargoCompanyDto>();
    }

    public async Task CreateAsync(CreateCargoCompanyDto dto)
    {
        var resp = await _httpClient.PostAsJsonAsync("cargocompanies", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(UpdateCargoCompanyDto dto)
    {
        var resp = await _httpClient.PutAsJsonAsync("cargocompanies", dto);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var resp = await _httpClient.DeleteAsync($"cargocompanies?id={id}");
        resp.EnsureSuccessStatusCode();
    }
}


