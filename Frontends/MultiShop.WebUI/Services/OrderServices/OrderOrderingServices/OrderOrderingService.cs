using MultiShop.WebUI.Dtos.OrderDtos.OrderOrderingDtos;

namespace MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;

public class OrderOrderingService : IOrderOrderingService
{
    private readonly HttpClient _httpClient;

    public OrderOrderingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<ResultOrderingByUserIdDto>> GetOrderingByUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new List<ResultOrderingByUserIdDto>();
        }
        var resp = await _httpClient.GetAsync($"orderings/GetOrderingByUserId/{userId}");
        if (!resp.IsSuccessStatusCode)
        {
            if ((int)resp.StatusCode == 404)
            {
                return new List<ResultOrderingByUserIdDto>();
            }
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve orderings. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
        return await resp.Content.ReadFromJsonAsync<List<ResultOrderingByUserIdDto>>()
               ?? new List<ResultOrderingByUserIdDto>();
    }

    public async Task CreateOrderingAsync(CreateOrderingDto dto)
    {
        var resp = await _httpClient.PostAsJsonAsync("orderings", dto);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create ordering. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }

    public async Task CreateOrderDetailAsync(CreateOrderDetailDto dto)
    {
        var resp = await _httpClient.PostAsJsonAsync("orderdetails", dto);
        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create order detail. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }
}
