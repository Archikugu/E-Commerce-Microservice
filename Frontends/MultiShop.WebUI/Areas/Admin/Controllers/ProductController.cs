using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    private async Task PopulateCategoriesAsync(string? selectedCategoryId = null)
    {
        var categories = await _categoryService.GetAllCategoryAsync();
        var items = categories.Select(c => new SelectListItem
        {
            Value = c.CategoryId,
            Text = c.CategoryName,
            Selected = selectedCategoryId != null && c.CategoryId == selectedCategoryId
        }).ToList();
        ViewBag.CategoryList = items;
        ViewBag.CategoryId = items; // bazı view'lar CategoryId anahtarını bekleyebilir
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Products";
        ViewBag.v3 = "Product List";

        var values = await _productService.GetProductsWithCategoryAsync();
        return View(values);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        await PopulateCategoriesAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(createProductDto.CategoryId);
            return View(createProductDto);
        }
        await _productService.CreateProductAsync(createProductDto);
        return RedirectToAction("Index", "Product", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateProduct(string id)
    {
        var read = await _productService.GetByIdProductAsync(id);
        var model = new UpdateProductDto
        {
            ProductId = read.ProductId,
            ProductName = read.ProductName,
            CategoryId = read.CategoryId,
            Price = read.Price,
            Description = read.Description,
            ImageUrl = read.ImageUrl
        };
        await PopulateCategoriesAsync(model.CategoryId);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(updateProductDto.CategoryId);
            return View(updateProductDto);
        }
        await _productService.UpdateProductAsync(updateProductDto);
        return RedirectToAction("Index", "Product", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        await _productService.DeleteProductAsync(id);
        return RedirectToAction("Index", "Product", new { area = "Admin" });
    }
}
