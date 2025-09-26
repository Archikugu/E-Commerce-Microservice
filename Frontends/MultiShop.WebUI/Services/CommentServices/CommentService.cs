using System.Text;
using MultiShop.WebUI.Dtos.CommentDtos;

namespace MultiShop.WebUI.Services.CommentServices
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;

        public CommentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ResultCommentDto>> GetAllAsync()
        {
            var resp = await _httpClient.GetAsync("comments");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve comments. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultCommentDto>>() ?? new List<ResultCommentDto>();
        }

        public async Task<List<ResultCommentDto>> GetByProductIdAsync(string productId)
        {
            var resp = await _httpClient.GetAsync($"comments/product/{productId}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve product comments. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            return await resp.Content.ReadFromJsonAsync<List<ResultCommentDto>>() ?? new List<ResultCommentDto>();
        }

        public async Task<ResultCommentDto> GetByIdAsync(string id)
        {
            var resp = await _httpClient.GetAsync($"comments/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to retrieve comment. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
            var value = await resp.Content.ReadFromJsonAsync<ResultCommentDto>();
            return value ?? throw new Exception("Comment not found.");
        }

        public async Task CreateAsync(CreateCommentDto dto)
        {
            var resp = await _httpClient.PostAsJsonAsync("comments", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create comment. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateAsync(UpdateCommentDto dto)
        {
            var resp = await _httpClient.PutAsJsonAsync($"comments/{dto.UserCommentId}", dto);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update comment. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"comments/{id}");
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to delete comment. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }

        public async Task UpdateStatusAsync(string id, bool status)
        {
            var content = new StringContent(status ? "true" : "false", Encoding.UTF8, "application/json");
            var resp = await _httpClient.PatchAsync($"comments/{id}/Status", content);
            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new Exception($"Failed to update comment status. Status: {(int)resp.StatusCode} {resp.StatusCode}. Content: {error}");
            }
        }
    }
}
