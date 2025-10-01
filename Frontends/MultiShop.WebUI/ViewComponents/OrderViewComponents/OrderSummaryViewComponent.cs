using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.BasketServices;

namespace MultiShop.WebUI.ViewComponents.OrderViewComponents;

public class OrderSummaryViewComponent : ViewComponent
{
    private readonly IBasketService _basketService;

    public OrderSummaryViewComponent(IBasketService basketService)
    {
        _basketService = basketService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var basket = await _basketService.GetBasket();
        return View(basket);
    }
}
