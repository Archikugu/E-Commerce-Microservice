using System.Net.Http.Json;

namespace MultiShop.WebUI.Services.StatisticsServices.DiscountStatisticServices;

public class DiscountStatisticsService : IDiscountStatisticsService
{
    private readonly HttpClient _httpClient;

    public DiscountStatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<int> GetTotalDiscountCouponCountAsync() => GetAsync<int>("Discounts/total-count");
    public Task<int> GetActiveDiscountCouponCountAsync() => GetAsync<int>("Discounts/active-count");
    public Task<int> GetInactiveDiscountCouponCountAsync() => GetAsync<int>("Discounts/inactive-count");
    public Task<int> GetExpiredDiscountCouponCountAsync() => GetAsync<int>("Discounts/expired-count");
    public Task<int> GetNonExpiredDiscountCouponCountAsync() => GetAsync<int>("Discounts/non-expired-count");

    private async Task<T> GetAsync<T>(string relativePath)
    {
        var resp = await _httpClient.GetAsync(relativePath);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Empty response body");
    }
}


