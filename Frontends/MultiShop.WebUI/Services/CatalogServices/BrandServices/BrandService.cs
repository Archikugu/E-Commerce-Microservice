using MultiShop.WebUI.Dtos.CatalogDtos.BrandDtos;

namespace MultiShop.WebUI.Services.CatalogServices.BrandServices
{
    public class BrandService : IBrandService
    {
        private readonly HttpClient _httpClient;

        public BrandService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultBrandDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("brands");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve brands. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultBrandDto>>() ?? new List<ResultBrandDto>();
        }

        public async Task<GetByIdBrandDto> GetByIdAsync(string id)
        {
            var resp = await _httpClient.GetAsync($"brands/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve brand. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<GetByIdBrandDto>();
            return value ?? throw new Exception("Brand not found.");
        }

        public async Task CreateAsync(CreateBrandDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("brands", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create brand. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateBrandDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync("brands", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update brand. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"brands/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete brand. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }
    }
}
