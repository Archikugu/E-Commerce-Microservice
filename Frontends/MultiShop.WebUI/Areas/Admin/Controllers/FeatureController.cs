using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.FeatureDtos;
using MultiShop.WebUI.Services.CatalogServices.FeatureServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class FeatureController : Controller
{
    private readonly IFeatureService _featureService;

    public FeatureController(IFeatureService featureService)
    {
        _featureService = featureService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Features";
        ViewBag.v3 = "Feature List";

        var values = await _featureService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateFeature()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeature(CreateFeatureDto createFeatureDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createFeatureDto);
        }
        await _featureService.CreateAsync(createFeatureDto);
        return RedirectToAction("Index", "Feature", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateFeature(string id)
    {
        var read = await _featureService.GetByIdAsync(id);
        var model = new UpdateFeatureDto
        {
            FeatureId = read.FeatureId,
            Title = read.Title,
            Icon = read.Icon,
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFeature(UpdateFeatureDto updateFeatureDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateFeatureDto);
        }
        await _featureService.UpdateAsync(updateFeatureDto);
        return RedirectToAction("Index", "Feature", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteFeature(string id)
    {
        await _featureService.DeleteAsync(id);
        return RedirectToAction("Index", "Feature", new { area = "Admin" });
    }
}
