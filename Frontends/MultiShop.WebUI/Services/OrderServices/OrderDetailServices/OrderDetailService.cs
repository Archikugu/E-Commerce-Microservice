using MultiShop.WebUI.Dtos.OrderDtos.OrderDetailDtos;

namespace MultiShop.WebUI.Services.OrderServices.OrderDetailServices;

public class OrderDetailService : IOrderDetailService
{
    private readonly HttpClient _httpClient;
    public OrderDetailService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ResultOrderDetailDto>> GetAllAsync()
    {
        var resp = await _httpClient.GetAsync("orderdetails");
        if (!resp.IsSuccessStatusCode)
        {
            if ((int)resp.StatusCode == 404)
            {
                return new List<ResultOrderDetailDto>();
            }
            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve order details. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
        return await resp.Content.ReadFromJsonAsync<List<ResultOrderDetailDto>>()
               ?? new List<ResultOrderDetailDto>();
    }

    public async Task<List<ResultOrderDetailDto>> GetByOrderingIdAsync(int orderingId)
    {
        var all = await GetAllAsync();
        return all.Where(x => x.OrderingId == orderingId).ToList();
    }
}


