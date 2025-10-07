using MultiShop.WebUI.Dtos.MessageDtos;
using MultiShop.WebUI.Settings;

namespace MultiShop.WebUI.Services.MessageServices;

public class MessageService : IMessageService
{
    private readonly HttpClient _httpClient;

    public MessageService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultMessageDto?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<ResultMessageDto>($"messages/{id}");
    }

    public async Task<List<ResultInboxMessageDto>> GetInboxAsync(string receiverId)
    {
        var res = await _httpClient.GetAsync($"messages/inbox/{receiverId}");
        if (!res.IsSuccessStatusCode) return new List<ResultInboxMessageDto>();
        return await res.Content.ReadFromJsonAsync<List<ResultInboxMessageDto>>() ?? new List<ResultInboxMessageDto>();
    }

    public async Task<List<ResultSendboxMessageDto>> GetSendboxAsync(string senderId)
    {
        var res = await _httpClient.GetAsync($"messages/sendbox/{senderId}");
        if (!res.IsSuccessStatusCode) return new List<ResultSendboxMessageDto>();
        return await res.Content.ReadFromJsonAsync<List<ResultSendboxMessageDto>>() ?? new List<ResultSendboxMessageDto>();
    }

    public async Task CreateAsync(CreateMessageDto dto)
    {
        var res = await _httpClient.PostAsJsonAsync("messages", dto);
        res.EnsureSuccessStatusCode();
    }

    public async Task UpdateAsync(UpdateMessageDto dto)
    {
        var res = await _httpClient.PutAsJsonAsync("messages", dto);
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id)
    {
        var res = await _httpClient.DeleteAsync($"messages/{id}");
        res.EnsureSuccessStatusCode();
    }

    public async Task MarkAsReadAsync(int id)
    {
        var res = await _httpClient.PostAsync($"messages/{id}/read", null);
        res.EnsureSuccessStatusCode();
    }
}


