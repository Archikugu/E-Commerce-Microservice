using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.BasketDtos;
using MultiShop.WebUI.Services.BasketServices;

namespace MultiShop.WebUI.ViewComponents.ShoppingCartViewComponents
{
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly IBasketService _basketService;

        public CartSummaryViewComponent(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            BasketTotalDto basket = await _basketService.GetBasket();
            basket ??= new BasketTotalDto { BasketItems = new List<BasketItemDto>(), DiscountCode = null, DiscountRate = null };
            return View(basket);
        }
    }
}
