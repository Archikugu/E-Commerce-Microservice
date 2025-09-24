using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;

namespace MultiShop.WebUI.Services.CatalogServices.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultProductDto>> GetAllProductAsync()
        {
            var response = await _httpClient.GetAsync("products");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve products. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
            var values = await response.Content.ReadFromJsonAsync<List<ResultProductDto>>();
            return values ?? new List<ResultProductDto>();
        }

        public async Task<GetByIdProductDto> GetByIdProductAsync(string id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve product. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
            var value = await response.Content.ReadFromJsonAsync<GetByIdProductDto>();
            return value ?? throw new Exception("Product not found.");
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var response = await _httpClient.PostAsJsonAsync("products", createProductDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create product. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var response = await _httpClient.PutAsJsonAsync("products", updateProductDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update product. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteProductAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete product. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryAsync()
        {
            var response = await _httpClient.GetAsync("products/GetProductsWithCategory");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve products with category. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
            var values = await response.Content.ReadFromJsonAsync<List<ResultProductsWithCategoryDto>>();
            return values ?? new List<ResultProductsWithCategoryDto>();
        }

        public async Task<List<ResultProductsWithCategoryDto>> GetProductsWithCategoryByCategoryIdAsync(string categoryId)
        {
            var response = await _httpClient.GetAsync($"products/GetProductsWithCategoryByCategoryId/{categoryId}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve products by category. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
            var values = await response.Content.ReadFromJsonAsync<List<ResultProductsWithCategoryDto>>();
            return values ?? new List<ResultProductsWithCategoryDto>();
        }
    }
}
