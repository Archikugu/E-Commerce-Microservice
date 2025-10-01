using MultiShop.WebUI.Dtos.OrderDtos.OrderAddressDtos;

namespace MultiShop.WebUI.Services.OrderServices.OrderAddressServices;

public class OrderAddressService : IOrderAddressService
{
    private readonly HttpClient _httpClient;

    public OrderAddressService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResultOrderAddressDto>> GetAllAsync()
    {
        var resp = await _httpClient.GetAsync("addresses");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve order addresses. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
        return await resp.Content.ReadFromJsonAsync<List<ResultOrderAddressDto>>()
               ?? new List<ResultOrderAddressDto>();
    }

    public async Task<GetByIdOrderAddressDto> GetByIdAsync(int id)
    {
        var resp = await _httpClient.GetAsync($"addresses/{id}");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve order address. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
        var value = await resp.Content.ReadFromJsonAsync<GetByIdOrderAddressDto>();
        return value ?? throw new Exception("Order address not found.");
    }

    public async Task CreateAsync(CreateOrderAddressDto dto)
    {
        var resp = await _httpClient.PostAsJsonAsync("addresses", dto);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create order address. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }

    public async Task UpdateAsync(UpdateOrderAddressDto dto)
    {
        var resp = await _httpClient.PutAsJsonAsync("addresses", dto);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update order address. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }

    public async Task DeleteAsync(int id)
    {
        var resp = await _httpClient.DeleteAsync($"addresses?id={id}");
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete order address. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }
}
