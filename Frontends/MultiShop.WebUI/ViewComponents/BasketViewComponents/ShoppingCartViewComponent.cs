using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.BasketServices;

namespace MultiShop.WebUI.ViewComponents.ShoppingCartViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;
        public ShoppingCartViewComponent(IBasketService basketService)
        {
            _basketService = basketService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var basket = await _basketService.GetBasket();
            return View(basket);
        }
    }
}

