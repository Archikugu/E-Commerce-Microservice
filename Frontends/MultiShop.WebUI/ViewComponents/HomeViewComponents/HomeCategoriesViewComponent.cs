using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents;

public class HomeCategoriesViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public HomeCategoriesViewComponent(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var categories = await _categoryService.GetAllCategoryAsync();

        // Her kategori için ürün sayısını getir (Ocelot + token üzerinden)
        foreach (var category in categories)
        {
            var products = await _productService.GetProductsWithCategoryByCategoryIdAsync(category.CategoryId);
            category.ProductCount = products?.Count ?? 0;
        }

        return View(categories);
    }
}
