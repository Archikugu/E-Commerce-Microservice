namespace MultiShop.SignalR.Services.SignalRMessageServices
{
    public class SignalRMessageService : ISignalRMessageService
    {
        private readonly HttpClient _httpClient;

        public SignalRMessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> GetTotalMessageCountReciverIdAsync(string id)
        {
            var res = await _httpClient.GetAsync($"messages/total-count-reciver-id/{id}");
            if (!res.IsSuccessStatusCode)
            {
                return 0;
            }
            var count = await res.Content.ReadFromJsonAsync<int>();
            return count;
        }

        public async Task<int> GetTotalMessageCountAsync()
        {
            var res = await _httpClient.GetAsync("messages/total-count");
            if (!res.IsSuccessStatusCode)
            {
                return 0;
            }
            var count = await res.Content.ReadFromJsonAsync<int>();
            return count;
        }
    }
}
