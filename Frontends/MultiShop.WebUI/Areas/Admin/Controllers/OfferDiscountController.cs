using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.OfferDiscountsDtos;
using MultiShop.WebUI.Services.CatalogServices.OfferDiscountServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class OfferDiscountController : Controller
{
    private readonly IOfferDiscountService _offerDiscountService;

    public OfferDiscountController(IOfferDiscountService offerDiscountService)
    {
        _offerDiscountService = offerDiscountService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Offer Discounts";
        ViewBag.v3 = "Offer Discount List";

        var values = await _offerDiscountService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateOfferDiscount()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateOfferDiscount(CreateOfferDiscountDto createOfferDiscountDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createOfferDiscountDto);
        }
        await _offerDiscountService.CreateAsync(createOfferDiscountDto);
        return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateOfferDiscount(string id)
    {
        var read = await _offerDiscountService.GetByIdAsync(id);
        var model = new UpdateOfferDiscountDto
        {
            OfferDiscountId = read.OfferDiscountId,
            Title = read.Title,
            SubTitle = read.SubTitle,
            ImageUrl = read.ImageUrl,
            ButtonTitle = read.ButtonTitle,
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOfferDiscount(UpdateOfferDiscountDto updateOfferDiscountDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateOfferDiscountDto);
        }
        await _offerDiscountService.UpdateAsync(updateOfferDiscountDto);
        return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteOfferDiscount(string id)
    {
        await _offerDiscountService.DeleteAsync(id);
        return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
    }
}
