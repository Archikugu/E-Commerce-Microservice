using System.Text;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using Newtonsoft.Json;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    public CategoryController(IHttpClientFactory httpClientFactory, ICategoryService categoryService, IProductService productService)
    {
        _httpClientFactory = httpClientFactory;
        _categoryService = categoryService;
        _productService = productService;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Categories";
        ViewBag.v3 = "Category List";

        var values = await _categoryService.GetAllCategoryAsync();
        return View(values);
    }
    [HttpGet]
    public async Task<IActionResult> CreateCategory()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        await _categoryService.CreateCategoryAsync(createCategoryDto);

        return RedirectToAction("Index", "Category", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateCategory(string id)
    {
        var read = await _categoryService.GetByIdCategoryAsync(id);
        var model = new UpdateCategoryDto
        {
            CategoryId = read.CategoryId,
            CategoryName = read.CategoryName,
            ImageUrl = read.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
    {
       await _categoryService.UpdateCategoryAsync(updateCategoryDto);
        return RedirectToAction("Index", "Category", new { area = "Admin" });
     
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return RedirectToAction("Index", "Category", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> ViewProducts(string id)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Categories";
        ViewBag.v3 = "Products in Category";
        // Kategori adını al
        var category = await _categoryService.GetByIdCategoryAsync(id);
        ViewBag.CategoryName = category.CategoryName;

        // Kategorideki ürünleri getir (Ocelot + token üzerinden)
        var values = await _productService.GetProductsWithCategoryByCategoryIdAsync(id);
        return View(values);
    }
}
