using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.FeatureSliderDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeCarouselViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeCarouselViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7001/api/FeatureSliders");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultFeatureSliderDto>>(jsonData);
                
                // Sadece aktif olan slider'ları filtrele
                var activeValues = values.Where(x => x.Status).ToList();
                
                // Maksimum 5 slider göster
                var limitedValues = activeValues.Take(5).ToList();
                ViewBag.SliderCount = limitedValues.Count;
                
                return View(limitedValues);
            }
            return View();
        }
    }
}
