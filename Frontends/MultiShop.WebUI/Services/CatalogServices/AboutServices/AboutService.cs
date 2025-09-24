using MultiShop.WebUI.Dtos.CatalogDtos.AboutDtos;

namespace MultiShop.WebUI.Services.CatalogServices.AboutServices
{
    public class AboutService : IAboutService
    {
        private readonly HttpClient _httpClient;

        public AboutService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultAboutDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("abouts");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve abouts. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultAboutDto>>() ?? new List<ResultAboutDto>();
        }

        public async Task<ResultAboutDto> GetByIdAsync(string id)
        {
            var resp = await _httpClient.GetAsync($"abouts/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve about. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<ResultAboutDto>();
            return value ?? throw new Exception("About not found.");
        }

        public async Task CreateAsync(CreateAboutDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("abouts", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create about. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateAboutDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync("abouts", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update about. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"abouts/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete about. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }
    }
}
