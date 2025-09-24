using MultiShop.WebUI.Dtos.CatalogDtos.FeatureSliderDtos;

namespace MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices
{
    public class FeatureSliderService : IFeatureSliderService
    {
        private readonly HttpClient _httpClient;

        public FeatureSliderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultFeatureSliderDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("featuresliders");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve feature sliders. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultFeatureSliderDto>>() ?? new List<ResultFeatureSliderDto>();
        }

        public async Task<ResultFeatureSliderDto> GetByIdAsync(string id)
        {
            var resp = await _httpClient.GetAsync($"featuresliders/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve feature slider. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<ResultFeatureSliderDto>();
            return value ?? throw new Exception("Feature slider not found.");
        }

        public async Task CreateAsync(CreateFeatureSliderDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("featuresliders", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create feature slider. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateFeatureSliderDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync("featuresliders", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update feature slider. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"featuresliders/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete feature slider. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task ChangeStatusTrueAsync(string id)
        {
            var resp = await _httpClient.PutAsync($"featuresliders/ChangeStatusTrue/{id}", null);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to set status true. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task ChangeStatusFalseAsync(string id)
        {
            var resp = await _httpClient.PutAsync($"featuresliders/ChangeStatusFalse/{id}", null);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to set status false. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task ToggleAsync(string id)
        {
            var cur = await GetByIdAsync(id);
            if (cur.Status)
                await ChangeStatusFalseAsync(id);
            else
                await ChangeStatusTrueAsync(id);
        }
    }
}
