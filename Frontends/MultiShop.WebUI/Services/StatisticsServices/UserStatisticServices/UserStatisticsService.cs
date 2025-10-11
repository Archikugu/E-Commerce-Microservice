using System.Net.Http.Json;

namespace MultiShop.WebUI.Services.StatisticsServices.UserStatistic;

public class UserStatisticsService : IUserStatisticsService
{
    private readonly HttpClient _httpClient;

    public UserStatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<long> GetUserCountAsync()
    {
        var resp = await _httpClient.GetAsync("api/Statistics/user-count");
        resp.EnsureSuccessStatusCode();
        var value = await resp.Content.ReadFromJsonAsync<long>();
        return value;
    }
}


