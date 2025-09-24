using MultiShop.WebUI.Dtos.CatalogDtos.FeatureDtos;

namespace MultiShop.WebUI.Services.CatalogServices.FeatureServices
{
    public class FeatureService : IFeatureService
    {
        private readonly HttpClient _httpClient;

        public FeatureService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultFeatureDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("features");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve features. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultFeatureDto>>() ?? new List<ResultFeatureDto>();
        }

        public async Task<GetByIdFeatureDto> GetByIdAsync(string id)
        {
            var resp = await _httpClient.GetAsync($"features/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve feature. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<GetByIdFeatureDto>();
            return value ?? throw new Exception("Feature not found.");
        }

        public async Task CreateAsync(CreateFeatureDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("features", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create feature. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateFeatureDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync("features", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update feature. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"features/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete feature. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }
    }
}
