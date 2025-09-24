using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.SpecialOfferDtos;
using MultiShop.WebUI.Services.CatalogServices.SpecialOfferServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class SpecialOfferController : Controller
{
    private readonly ISpecialOfferService _specialOfferService;

    public SpecialOfferController(ISpecialOfferService specialOfferService)
    {
        _specialOfferService = specialOfferService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Special Offer";
        ViewBag.v3 = "Special Offer List";

        var values = await _specialOfferService.GetAllSpecialOfferAsync();
        return View(values);
    }

    [HttpGet]
    public async Task<IActionResult> CreateSpecialOffer()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpecialOffer(CreateSpecialOfferDto createSpecialOfferDto)
    {
        await _specialOfferService.CreateSpecialOfferAsync(createSpecialOfferDto);
        return RedirectToAction("Index", "SpecialOffer", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateSpecialOffer(string id)
    {
        var value = await _specialOfferService.GetByIdSpecialOfferAsync(id);
        var model = new UpdateSpecialOfferDto
        {
            SpecialOfferId = value.SpecialOfferId,
            Title = value.Title,
            SubTitle = value.SubTitle,
            ImageUrl = value.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSpecialOffer(UpdateSpecialOfferDto updateSpecialOfferDto)
    {
        await _specialOfferService.UpdateSpecialOfferAsync(updateSpecialOfferDto);
        return RedirectToAction("Index", "SpecialOffer", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteSpecialOffer(string id)
    {
        await _specialOfferService.DeleteSpecialOfferAsync(id);
        return RedirectToAction("Index", "SpecialOffer", new { area = "Admin" });
    }
}
