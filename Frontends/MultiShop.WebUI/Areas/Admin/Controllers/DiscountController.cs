using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.DiscountDtos.DiscountCouponDtos;
using MultiShop.WebUI.Services.DiscountServices.DiscountCouponServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class DiscountController : Controller
{
    private readonly IDiscountCouponService _discountService;

    public DiscountController(IDiscountCouponService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Discounts";
        ViewBag.v3 = "Discount Coupon List";

        var values = await _discountService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDiscountCouponDto createDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createDto);
        }
        await _discountService.CreateAsync(createDto);
        return RedirectToAction("Index", "Discount", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var read = await _discountService.GetByIdAsync(id);
        var model = new UpdateDiscountCouponDto
        {
            CouponId = read.CouponId,
            Code = read.Code,
            Rate = read.Rate,
            IsActive = read.IsActive,
            ValidDate = read.ValidDate
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateDiscountCouponDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateDto);
        }
        await _discountService.UpdateAsync(updateDto);
        return RedirectToAction("Index", "Discount", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        await _discountService.DeleteAsync(id);
        return RedirectToAction("Index", "Discount", new { area = "Admin" });
    }
}


