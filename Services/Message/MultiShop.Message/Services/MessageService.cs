using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MultiShop.Message.DataAccess.Contexts;
using MultiShop.Message.Dtos;

namespace MultiShop.Message.Services;

public class MessageService : IMessageService
{
    private readonly MultiShopMessageDbContext _multiShopMessageDbContext;
    private readonly IMapper _mapper;

    public MessageService(MultiShopMessageDbContext multiShopMessageDbContext, IMapper mapper)
    {
        _multiShopMessageDbContext = multiShopMessageDbContext;
        _mapper = mapper;
    }

    public async Task CreateAsync(CreateMessageDto dto)
    {
        var entity = _mapper.Map<DataAccess.Entities.Message>(dto);
        _multiShopMessageDbContext.Messages.Add(entity);
        await _multiShopMessageDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateMessageDto dto)
    {
        var entity = await _multiShopMessageDbContext.Messages.FindAsync(dto.MessageId);
        if (entity == null) return;
        _mapper.Map(dto, entity);
        await _multiShopMessageDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int messageId)
    {
        var entity = await _multiShopMessageDbContext.Messages.FindAsync(messageId);
        if (entity == null) return;
        _multiShopMessageDbContext.Messages.Remove(entity);
        await _multiShopMessageDbContext.SaveChangesAsync();
    }

    public async Task<ResultMessageDto> GetByIdAsync(int messageId)
    {
        var m = await _multiShopMessageDbContext.Messages.AsNoTracking().FirstOrDefaultAsync(x => x.MessageId == messageId);
        if (m == null) return null;
        return _mapper.Map<ResultMessageDto>(m);
    }

    public async Task<List<ResultInboxMessageDto>> GetInboxAsync(string receiverId)
    {
        return await _multiShopMessageDbContext.Messages.AsNoTracking()
            .Where(x => x.ReceiverId == receiverId)
            .OrderByDescending(x => x.MessageDate)
            .Select(m => _mapper.Map<ResultInboxMessageDto>(m))
            .ToListAsync();
    }

    public async Task<List<ResultSendboxMessageDto>> GetSendboxAsync(string senderId)
    {
        return await _multiShopMessageDbContext.Messages.AsNoTracking()
            .Where(x => x.SenderId == senderId)
            .OrderByDescending(x => x.MessageDate)
            .Select(m => _mapper.Map<ResultSendboxMessageDto>(m))
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int messageId)
    {
        var entity = await _multiShopMessageDbContext.Messages.FindAsync(messageId);
        if (entity == null) return;
        if (!entity.IsRead)
        {
            entity.IsRead = true;
            await _multiShopMessageDbContext.SaveChangesAsync();
        }
    }
}


