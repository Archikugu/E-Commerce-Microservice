using System.Threading.Tasks;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;

namespace MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;

public interface ICatalogStatisticsService
{
    Task<long> GetCategoryCountAsync();
    Task<long> GetProductCountAsync();
    Task<long> GetBrandCountAsync();
    Task<decimal> GetAverageProductPriceAsync();
    Task<T> GetAsync<T>(string relativePath);
    Task<ResultProductDto> GetCheapestProductAsync();
    Task<ResultProductDto> GetMostExpensiveProductAsync();
}


