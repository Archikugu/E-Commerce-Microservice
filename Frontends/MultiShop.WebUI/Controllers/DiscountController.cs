using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.DiscountServices.DiscountCouponServices;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Dtos.BasketDtos;

namespace MultiShop.WebUI.Controllers;

[Authorize]
public class DiscountController : Controller
{
    private readonly IDiscountCouponService _discountService;
    private readonly IBasketService _basketService;
    public DiscountController(IDiscountCouponService discountService, IBasketService basketService)
    {
        _discountService = discountService;
        _basketService = basketService;
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmDiscountCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            // Kupon opsiyonel: boşsa indirimi kaldır ve sepete dön
            try
            {
                var basket = await _basketService.GetBasket();
                if (basket != null)
                {
                    basket.DiscountCode = null;
                    basket.DiscountRate = null;
                    await _basketService.SaveBasket(basket);
                }
            TempData["SuccessMessage"] = "Coupon removed.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error while removing coupon: {ex.Message}";
            }
            return RedirectToAction("Index", "Basket");
        }
        try
        {
            var coupon = await _discountService.GetByCodeAsync(code);
            if (coupon == null)
            {
                TempData["ErrorMessage"] = "Invalid or expired coupon.";
                return RedirectToAction("Index", "Basket");
            }
            if (!coupon.IsActive || coupon.ValidDate.Date < DateTime.UtcNow.Date)
            {
                TempData["ErrorMessage"] = "Coupon is invalid or expired.";
                return RedirectToAction("Index", "Basket");
            }

            var basket = await _basketService.GetBasket();
            if (basket == null)
            {
                basket = new BasketTotalDto
                {
                    DiscountCode = code,
                    DiscountRate = coupon.Rate,
                    BasketItems = new List<BasketItemDto>()
                };
            }
            else
            {
                basket.DiscountCode = code;
                basket.DiscountRate = coupon.Rate;
            }

            await _basketService.SaveBasket(basket);
            TempData["SuccessMessage"] = "Coupon applied.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Coupon verification error: {ex.Message}";
        }

        return RedirectToAction("Index", "Basket");
    }
}


