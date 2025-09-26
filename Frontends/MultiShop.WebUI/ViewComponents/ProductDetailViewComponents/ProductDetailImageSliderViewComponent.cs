using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductImageServices;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponents
{
    public class ProductDetailImageSliderViewComponent : ViewComponent
    {
        private readonly IProductImageService _productImageService;

        public ProductDetailImageSliderViewComponent(IProductImageService productImageService)
        {
            _productImageService = productImageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var sliderDto = await _productImageService.GetProductImageSliderByProductIdAsync(id);
            return View(sliderDto ?? new ProductImageSliderDto
            {
                ProductId = id,
                ImageUrls = new List<string>(),
                TotalImageCount = 0
            });
        }
    }
}
