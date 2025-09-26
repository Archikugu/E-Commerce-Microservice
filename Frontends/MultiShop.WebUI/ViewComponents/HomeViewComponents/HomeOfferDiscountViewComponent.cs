using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.OfferDiscountsDtos;
using MultiShop.WebUI.Services.CatalogServices.OfferDiscountServices;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeOfferDiscountViewComponent : ViewComponent
    {
        private readonly IOfferDiscountService _offerDiscountService;

        public HomeOfferDiscountViewComponent(IOfferDiscountService offerDiscountService)
        {
            _offerDiscountService = offerDiscountService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var values = await _offerDiscountService.GetAllAsync();
            return View(values);
        }
    }
}
