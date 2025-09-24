using MultiShop.WebUI.Dtos.CatalogDtos.OfferDiscountsDtos;

namespace MultiShop.WebUI.Services.CatalogServices.OfferDiscountServices
{
    public class OfferDiscountService : IOfferDiscountService
    {
        private readonly HttpClient _httpClient;

        public OfferDiscountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultOfferDiscountDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("offerdiscounts");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve offer discounts. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultOfferDiscountDto>>() ?? new List<ResultOfferDiscountDto>();
        }

        public async Task<GetByIdOfferDiscountDto> GetByIdAsync(string id)
        {
            var resp = await _httpClient.GetAsync($"offerdiscounts/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve offer discount. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<GetByIdOfferDiscountDto>();
            return value ?? throw new Exception("Offer discount not found.");
        }

        public async Task CreateAsync(CreateOfferDiscountDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("offerdiscounts", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create offer discount. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateOfferDiscountDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync("offerdiscounts", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update offer discount. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"offerdiscounts/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete offer discount. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }
    }
}
