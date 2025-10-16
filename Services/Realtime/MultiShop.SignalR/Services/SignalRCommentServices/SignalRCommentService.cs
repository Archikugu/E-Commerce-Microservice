namespace MultiShop.SignalR.Services.SignalRCommentServices
{
    public class SignalRCommentService : ISignalRCommentService
    {
        private readonly HttpClient _httpClient;

        public SignalRCommentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> GetTotalCommentCountAsync()
        {
            // Ocelot upstream: /services/comment/{everything} → downstream: /api/{everything}
            // Controller route: api/Comments/total-count
            var res = await _httpClient.GetAsync("Comments/total-count");
            if (!res.IsSuccessStatusCode)
            {
                return 0;
            }
            var count = await res.Content.ReadFromJsonAsync<int>();
            return count;
        }
    }
}
