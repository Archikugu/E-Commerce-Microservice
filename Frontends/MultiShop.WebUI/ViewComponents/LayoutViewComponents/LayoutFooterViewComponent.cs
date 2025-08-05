using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.AboutDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents
{
    public class LayoutFooterViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LayoutFooterViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync("https://localhost:7001/api/Abouts");
            
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var aboutList = JsonConvert.DeserializeObject<List<ResultAboutDto>>(jsonData);
                return View(aboutList);
            }

            return View(new List<ResultAboutDto>());
        }
    }
}
