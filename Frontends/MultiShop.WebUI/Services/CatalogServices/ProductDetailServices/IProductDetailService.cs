using MultiShop.WebUI.Dtos.CatalogDtos.ProductDetailDtos;

namespace MultiShop.WebUI.Services.CatalogServices.ProductDetailServices
{
    public interface IProductDetailService
    {
        Task<List<ResultProductDetailDto>> GetDetailsByProductIdAsync(string productId);
    }
}
