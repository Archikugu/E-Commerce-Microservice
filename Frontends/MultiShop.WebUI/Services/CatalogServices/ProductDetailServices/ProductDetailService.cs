using MultiShop.WebUI.Dtos.CatalogDtos.ProductDetailDtos;

namespace MultiShop.WebUI.Services.CatalogServices.ProductDetailServices
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly HttpClient _httpClient;

        public ProductDetailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultProductDetailDto>> GetDetailsByProductIdAsync(string productId)
        {
            var resp = await _httpClient.GetAsync($"productdetails/GetProductDetailsByProductId/{productId}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve product details. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultProductDetailDto>>() ?? new List<ResultProductDetailDto>();
        }
    }
}
