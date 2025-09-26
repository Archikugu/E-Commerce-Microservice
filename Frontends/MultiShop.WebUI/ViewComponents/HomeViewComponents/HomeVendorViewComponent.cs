using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.BrandDtos;
using MultiShop.WebUI.Services.CatalogServices.BrandServices;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents;

public class HomeVendorViewComponent : ViewComponent
{
    private readonly IBrandService _brandService;

    public HomeVendorViewComponent(IBrandService brandService)
    {
        _brandService = brandService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int maxCount = 12)
    {
        var values = await _brandService.GetAllAsync();
        var limited = values?.Take(maxCount).ToList() ?? new List<ResultBrandDto>();
        return View(limited);
    }
}
