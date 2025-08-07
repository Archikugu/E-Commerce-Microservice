using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDetailDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponent
{
    public class ProductDetailInformationViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductDetailInformationViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7001/api/ProductDetails/GetProductDetailsByProductId/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var details = JsonConvert.DeserializeObject<List<ResultProductDetailDto>>(jsonData);
                // İlk ProductDetail'ı al, yoksa null
                var productDetail = details?.FirstOrDefault();
                return View("Default", productDetail);
            }
            return View("Default", (ResultProductDetailDto)null);
        }
    }
}
