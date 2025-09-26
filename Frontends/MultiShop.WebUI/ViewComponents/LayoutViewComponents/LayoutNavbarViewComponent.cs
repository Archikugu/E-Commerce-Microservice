using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents;

public class LayoutNavbarViewComponent : ViewComponent
{
    private readonly ICategoryService _categoryService;

    public LayoutNavbarViewComponent(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var values = await _categoryService.GetAllCategoryAsync();
        return View(values);
    }
}