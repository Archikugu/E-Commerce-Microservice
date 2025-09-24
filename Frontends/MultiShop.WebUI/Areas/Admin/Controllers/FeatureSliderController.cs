using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.FeatureSliderDtos;
using MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class FeatureSliderController : Controller
{
    private readonly IFeatureSliderService _featureSliderService;

    public FeatureSliderController(IFeatureSliderService featureSliderService)
    {
        _featureSliderService = featureSliderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Feature Slider";
        ViewBag.v3 = "Feature Slider List";

        var values = await _featureSliderService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateFeatureSlider()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeatureSlider(CreateFeatureSliderDto createFeatureSliderDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createFeatureSliderDto);
        }
        await _featureSliderService.CreateAsync(createFeatureSliderDto);
        return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateFeatureSlider(string id)
    {
        var read = await _featureSliderService.GetByIdAsync(id);
        var model = new UpdateFeatureSliderDto
        {
            FeatureSliderId = read.FeatureSliderId,
            Title = read.Title,
            ImageUrl = read.ImageUrl,
            Description = read.Description,
            Status = read.Status
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFeatureSlider(UpdateFeatureSliderDto updateFeatureSliderDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateFeatureSliderDto);
        }
        await _featureSliderService.UpdateAsync(updateFeatureSliderDto);
        return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteFeatureSlider(string id)
    {
        await _featureSliderService.DeleteAsync(id);
        return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Toggle(string id, bool status)
    {
        if (status)
        {
            await _featureSliderService.ChangeStatusFalseAsync(id);
        }
        else
        {
            await _featureSliderService.ChangeStatusTrueAsync(id);
        }
        return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
    }
}
