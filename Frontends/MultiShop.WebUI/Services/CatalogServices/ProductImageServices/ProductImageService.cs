using MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;

namespace MultiShop.WebUI.Services.CatalogServices.ProductImageServices
{
    public class ProductImageService : IProductImageService
    {
        private readonly HttpClient _httpClient;

        public ProductImageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductImageSliderDto> GetProductImageSliderByProductIdAsync(string productId)
        {
            var resp = await _httpClient.GetAsync($"productimages/ProductImageSliderByProductId/{productId}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve product image slider. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var slider = await resp.Content.ReadFromJsonAsync<ProductImageSliderDto>();
            return slider ?? new ProductImageSliderDto { ProductId = productId, ImageUrls = new List<string>(), TotalImageCount = 0 };
        }
    }
}
