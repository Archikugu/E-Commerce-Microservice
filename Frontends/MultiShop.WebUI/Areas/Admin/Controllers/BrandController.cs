using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.BrandDtos;
using MultiShop.WebUI.Services.CatalogServices.BrandServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class BrandController : Controller
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Brands";
        ViewBag.v3 = "Brand List";

        var values = await _brandService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateBrand()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBrand(CreateBrandDto createBrandDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createBrandDto);
        }
        await _brandService.CreateAsync(createBrandDto);
        return RedirectToAction("Index", "Brand", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateBrand(string id)
    {
        var read = await _brandService.GetByIdAsync(id);
        var model = new UpdateBrandDto
        {
            BrandId = read.BrandId,
            BrandName = read.BrandName,
            ImageUrl = read.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBrand(UpdateBrandDto updateBrandDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateBrandDto);
        }
        await _brandService.UpdateAsync(updateBrandDto);
        return RedirectToAction("Index", "Brand", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteBrand(string id)
    {
        await _brandService.DeleteAsync(id);
        return RedirectToAction("Index", "Brand", new { area = "Admin" });
    }
} 