using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents;

public class HomeFeaturedProductsViewComponent : ViewComponent
{
    private readonly IProductService _productService;

    public HomeFeaturedProductsViewComponent(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int maxCount = 8)
    {
        var values = await _productService.GetAllProductAsync();
        var limited = values?.Take(maxCount).ToList() ?? new List<ResultProductDto>();
        return View(limited);
    }
}
