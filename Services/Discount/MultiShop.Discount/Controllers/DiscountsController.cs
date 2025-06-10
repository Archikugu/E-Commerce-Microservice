using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Discount.Dtos.DiscountCouponDtos;
using MultiShop.Discount.Services.DiscountServices;

namespace MultiShop.Discount.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DiscountsController : ControllerBase
{
    private readonly IDiscountService _discountService;
    public DiscountsController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    [HttpGet]
    public async Task<IActionResult> DiscountCouponList()
    {
        var values = await _discountService.GetAllDiscountCouponAsync();
        return Ok(values);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDiscountCouponById(int id)
    {
        var values = await _discountService.GetByIdDiscountCouponAsync(id);
        return Ok(values);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDiscountCoupon(CreateDiscountCouponDto createDiscountCouponDto)
    {
        await _discountService.CreateDiscountCouponAsync(createDiscountCouponDto);
        return Ok("Discount Coupon Successfully Created");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteDiscountCoupon(int id)
    {
        await _discountService.DeleteDiscountCouponAsync(id);
        return Ok("Discount Coupon Successfully Deleted");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDiscountCoupon(UpdateDiscountCouponDto updateDiscountCouponDto)
    {
        await _discountService.UpdateDiscountCouponAsync(updateDiscountCouponDto);
        return Ok("Discount Coupon Successfully Updated");
    }
}
