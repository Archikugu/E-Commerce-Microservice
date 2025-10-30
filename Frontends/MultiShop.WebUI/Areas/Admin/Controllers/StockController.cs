using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class StockController : Controller
{
    private readonly IProductService _productService;

    public StockController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        ViewBag.v1 = "Store";
        ViewBag.v2 = "Stock Management";
        ViewBag.v3 = "Stock List";

        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var all = await _productService.GetProductsWithCategoryAsync();
        var totalItems = all?.Count ?? 0;
        var items = (all ?? new List<MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos.ResultProductsWithCategoryDto>())
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;

        return View(items);
    }
}


