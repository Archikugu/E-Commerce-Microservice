using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponents
{
    public class ProductDetailImageSliderViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductDetailImageSliderViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7001/api/ProductImages/ProductImageSliderByProductId/{id}");
            
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var sliderDto = JsonConvert.DeserializeObject<ProductImageSliderDto>(jsonData);
                return View(sliderDto);
            }
            
            // Eğer ProductImage bulunamazsa boş DTO döndür
            return View(new ProductImageSliderDto
            {
                ProductId = id,
                ImageUrls = new List<string>(),
                TotalImageCount = 0
            });
        }
    }
}
