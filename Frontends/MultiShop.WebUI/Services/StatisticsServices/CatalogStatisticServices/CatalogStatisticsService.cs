using System.Net.Http.Json;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;

namespace MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;

public class CatalogStatisticsService : ICatalogStatisticsService
{
    private readonly HttpClient _httpClient;

    public CatalogStatisticsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<long> GetCategoryCountAsync() => GetAsync<long>("Statistics/category-count");
    public Task<long> GetProductCountAsync() => GetAsync<long>("Statistics/product-count");
    public Task<long> GetBrandCountAsync() => GetAsync<long>("Statistics/brand-count");
    public Task<decimal> GetAverageProductPriceAsync() => GetAsync<decimal>("Statistics/average-product-price");
    public Task<ResultProductDto> GetCheapestProductAsync() => GetAsync<ResultProductDto>("Statistics/cheapest-product");
    public Task<ResultProductDto> GetMostExpensiveProductAsync() => GetAsync<ResultProductDto>("Statistics/most-expensive-product");

    public async Task<T> GetAsync<T>(string relativePath)
    {
        var resp = await _httpClient.GetAsync(relativePath);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException("Empty response body");
    }
}


