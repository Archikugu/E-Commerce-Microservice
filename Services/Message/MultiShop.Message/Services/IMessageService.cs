using MultiShop.Message.Dtos;

namespace MultiShop.Message.Services;

public interface IMessageService
{
    Task CreateAsync(CreateMessageDto dto);
    Task UpdateAsync(UpdateMessageDto dto);
    Task DeleteAsync(int messageId);
    Task<ResultMessageDto> GetByIdAsync(int messageId);
    Task<List<ResultInboxMessageDto>> GetInboxAsync(string receiverId);
    Task<List<ResultSendboxMessageDto>> GetSendboxAsync(string senderId);
    Task MarkAsReadAsync(int messageId);
}


