using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Dtos.OrderDtos.OrderOrderingDtos;

namespace MultiShop.WebUI.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly IBasketService _basketService;
    private readonly ILoginService _loginService;
    private readonly IOrderOrderingService _orderOrderingService;

    public PaymentController(IBasketService basketService, ILoginService loginService, IOrderOrderingService orderOrderingService)
    {
        _basketService = basketService;
        _loginService = loginService;
        _orderOrderingService = orderOrderingService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Process()
    {
        try
        {
            var userId = _loginService.GetUserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { ok = false, message = "User session not found." });
            }

            var basket = await _basketService.GetBasket();
            if (basket == null || basket.BasketItems == null || !basket.BasketItems.Any())
            {
                return BadRequest(new { ok = false, message = "Basket is empty." });
            }

            var subtotal = basket.TotalPrice;
            var rate = basket.DiscountRate ?? 0;
            var total = rate > 0 ? subtotal * (100 - rate) / 100 : subtotal;

            var orderingId = await _orderOrderingService.CreateOrderingAsync(new CreateOrderingDto
            {
                UserId = userId,
                TotalPrice = total,
                OrderDate = DateTime.UtcNow
            });

            foreach (var item in basket.BasketItems)
            {
                await _orderOrderingService.CreateOrderDetailAsync(new CreateOrderDetailDto
                {
                    OrderingId = orderingId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductPrice = item.Price,
                    ProductAmount = item.Quantity,
                    ProductTotalPrice = item.Price * item.Quantity
                });
            }

            await _basketService.DeleteBasket(userId);

            return Json(new { ok = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { ok = false, message = ex.Message });
        }
    }
}
