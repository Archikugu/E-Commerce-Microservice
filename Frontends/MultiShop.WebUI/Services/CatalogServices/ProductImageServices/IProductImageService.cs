using MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;

namespace MultiShop.WebUI.Services.CatalogServices.ProductImageServices
{
    public interface IProductImageService
    {
        Task<ProductImageSliderDto> GetProductImageSliderByProductIdAsync(string productId);
    }
}
