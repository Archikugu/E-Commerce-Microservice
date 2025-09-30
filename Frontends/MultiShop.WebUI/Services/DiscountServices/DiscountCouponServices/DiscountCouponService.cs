using MultiShop.WebUI.Dtos.DiscountDtos.DiscountCouponDtos;

namespace MultiShop.WebUI.Services.DiscountServices.DiscountCouponServices
{
    public class DiscountCouponService : IDiscountCouponService
    {
        private readonly HttpClient _httpClient;

        public DiscountCouponService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultDiscountCouponDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("discounts");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve discount coupons. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultDiscountCouponDto>>() ?? new List<ResultDiscountCouponDto>();
        }

        public async Task<GetByIdDiscountCouponDto> GetByIdAsync(int id)
        {
            var resp = await _httpClient.GetAsync($"discounts/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve discount coupon. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<GetByIdDiscountCouponDto>();
            return value ?? throw new Exception("Discount coupon not found.");
        }

        public async Task<GetByIdDiscountCouponDto> GetByCodeAsync(string code)
        {
            var resp = await _httpClient.GetAsync($"discounts/by-code/{code}");
            if (!resp.IsSuccessStatusCode)
            {
                if ((int)resp.StatusCode == 404)
                {
                    return null; // invalid or not found
                }
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve discount coupon by code. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<GetByIdDiscountCouponDto>();
            return value ?? throw new Exception("Discount coupon not found.");
        }

        public async Task CreateAsync(CreateDiscountCouponDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("discounts", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create discount coupon. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateDiscountCouponDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync("discounts", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update discount coupon. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            // API [HttpDelete] bekliyor, parametre querystring olabilir: /api/discounts?id=1
            var resp = await _httpClient.DeleteAsync($"discounts?id={id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete discount coupon. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }
    }
}


