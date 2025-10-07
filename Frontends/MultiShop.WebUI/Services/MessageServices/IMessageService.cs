using MultiShop.WebUI.Dtos.MessageDtos;

namespace MultiShop.WebUI.Services.MessageServices;

public interface IMessageService
{
    Task<ResultMessageDto?> GetByIdAsync(int id);
    Task<List<ResultInboxMessageDto>> GetInboxAsync(string receiverId);
    Task<List<ResultSendboxMessageDto>> GetSendboxAsync(string senderId);
    Task CreateAsync(CreateMessageDto dto);
    Task UpdateAsync(UpdateMessageDto dto);
    Task DeleteAsync(int id);
    Task MarkAsReadAsync(int id);
}


