using System.Threading.Tasks;
using MultiShop.Catalog.Dtos.ProductDtos;

namespace MultiShop.Catalog.Services.StatisticServices;

public interface IStatisticService
{
    Task<long> GetCategoryCountAsync();
    Task<long> GetProductCountAsync();
    Task<long> GetBrandCountAsync();
    Task<decimal> GetAverageProductPriceAsync();
    Task<ResultProductDto> GetCheapestProductAsync();
    Task<ResultProductDto> GetMostExpensiveProductAsync();
}


