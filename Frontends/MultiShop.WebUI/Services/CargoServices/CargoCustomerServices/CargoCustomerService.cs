using MultiShop.WebUI.Dtos.CargoDtos.CargoCustomerDtos;

namespace MultiShop.WebUI.Services.CargoServices.CargoCustomerServices
{
    public class CargoCustomerService : ICargoCustomerService
    {
        private readonly HttpClient _httpClient;

        public CargoCustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GetCargoCustomerByIdDto> GetByIdCargoCustomerInfoAsync(string userCustomerId)
        {
            if (string.IsNullOrWhiteSpace(userCustomerId))
            {
                return new GetCargoCustomerByIdDto();
            }

            var resp = await _httpClient.GetAsync($"cargocustomers/ByUser/{Uri.EscapeDataString(userCustomerId)}");
            if (resp.IsSuccessStatusCode)
            {
                var itemOk = await resp.Content.ReadFromJsonAsync<GetCargoCustomerByIdDto>();
                return itemOk ?? new GetCargoCustomerByIdDto();
            }

            // Fallback: 404 ise tüm listeyi çekip client-side filtrele
            if ((int)resp.StatusCode == 404)
            {
                var listResp = await _httpClient.GetAsync("cargocustomers");
                if (!listResp.IsSuccessStatusCode)
                {
                    var listErr = await listResp.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to retrieve cargo customers. Status: {(int)listResp.StatusCode} {listResp.StatusCode}. Content: {listErr}");
                }
                var list = await listResp.Content.ReadFromJsonAsync<List<GetCargoCustomerByIdDto>>()
                           ?? new List<GetCargoCustomerByIdDto>();
                var normalizedTarget = userCustomerId.Trim();
                var item = list.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.UserCustomerId) &&
                    string.Equals(x.UserCustomerId.Trim(), normalizedTarget, StringComparison.OrdinalIgnoreCase));
                return item ?? new GetCargoCustomerByIdDto();
            }

            var error = await resp.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve cargo customer by user. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
        }
    }
}
