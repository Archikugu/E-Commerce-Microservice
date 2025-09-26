using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.FeatureSliderDtos;
using MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeCarouselViewComponent : ViewComponent
    {
        private readonly IFeatureSliderService _featureSliderService;

        public HomeCarouselViewComponent(IFeatureSliderService featureSliderService)
        {
            _featureSliderService = featureSliderService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int maxCount = 5)
        {
            var values = await _featureSliderService.GetAllAsync();

            var limitedValues = values
                .Where(x => x.Status)
                .Take(maxCount)
                .ToList();

            ViewBag.SliderCount = limitedValues.Count;

            return View(limitedValues);
        }
    }
}
