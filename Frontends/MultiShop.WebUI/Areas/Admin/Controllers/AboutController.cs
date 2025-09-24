using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.AboutDtos;
using MultiShop.WebUI.Services.CatalogServices.AboutServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class AboutController : Controller
{
    private readonly IAboutService _aboutService;

    public AboutController(IAboutService aboutService)
    {
        _aboutService = aboutService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "About";
        ViewBag.v3 = "About List";

        var values = await _aboutService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateAbout()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAbout(CreateAboutDto createAboutDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createAboutDto);
        }
        await _aboutService.CreateAsync(createAboutDto);
        return RedirectToAction("Index", "About", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateAbout(string id)
    {
        var read = await _aboutService.GetByIdAsync(id);
        var model = new UpdateAboutDto
        {
            AboutId = read.AboutId,
            Title = read.Title,
            ImageUrl = read.ImageUrl,
            Description = read.Description,
            Phone = read.Phone,
            Address = read.Address,
            Email = read.Email,
            WorkingHours = read.WorkingHours
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAbout(UpdateAboutDto updateAboutDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateAboutDto);
        }
        await _aboutService.UpdateAsync(updateAboutDto);
        return RedirectToAction("Index", "About", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteAbout(string id)
    {
        await _aboutService.DeleteAsync(id);
        return RedirectToAction("Index", "About", new { area = "Admin" });
    }
} 