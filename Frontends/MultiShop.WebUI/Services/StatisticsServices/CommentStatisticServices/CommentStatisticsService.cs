using System.Net.Http.Json;

namespace MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;

public class CommentStatisticsService : ICommentStatisticsService
{
    private readonly HttpClient _httpClient;

    public CommentStatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<int> GetActiveCommentCountAsync() => GetAsync<int>("Comments/active-count");
    public Task<int> GetPassiveCommentCountAsync() => GetAsync<int>("Comments/passive-count");
    public Task<int> GetTotalCommentCountAsync() => GetAsync<int>("Comments/total-count");

    private async Task<T> GetAsync<T>(string relativePath)
    {
        var resp = await _httpClient.GetAsync(relativePath);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Empty response body");
    }
}