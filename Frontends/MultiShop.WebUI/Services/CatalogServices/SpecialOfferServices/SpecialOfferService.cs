using MultiShop.WebUI.Dtos.CatalogDtos.SpecialOfferDtos;

namespace MultiShop.WebUI.Services.CatalogServices.SpecialOfferServices
{
    public class SpecialOfferService : ISpecialOfferService
    {
        private readonly HttpClient _httpClient;

        public SpecialOfferService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultSpecialOfferDto>> GetAllSpecialOfferAsync()
        {
            var response = await _httpClient.GetAsync("specialoffers");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve special offers. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
            var values = await response.Content.ReadFromJsonAsync<List<ResultSpecialOfferDto>>();
            return values ?? new List<ResultSpecialOfferDto>();
        }

        public async Task<GetByIdSpecialOfferDto> GetByIdSpecialOfferAsync(string id)
        {
            var response = await _httpClient.GetAsync($"specialoffers/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve special offer. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
            var value = await response.Content.ReadFromJsonAsync<GetByIdSpecialOfferDto>();
            return value ?? throw new Exception("Special offer not found.");
        }

        public async Task CreateSpecialOfferAsync(CreateSpecialOfferDto createSpecialOfferDto)
        {
            var response = await _httpClient.PostAsJsonAsync("specialoffers", createSpecialOfferDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create special offer. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateSpecialOfferAsync(UpdateSpecialOfferDto updateSpecialOfferDto)
        {
            var response = await _httpClient.PutAsJsonAsync("specialoffers", updateSpecialOfferDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update special offer. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteSpecialOfferAsync(string id)
        {
            var response = await _httpClient.DeleteAsync($"specialoffers/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete special offer. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
            }
        }
    }
}
