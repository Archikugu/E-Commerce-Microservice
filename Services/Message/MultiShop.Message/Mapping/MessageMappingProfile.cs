using AutoMapper;
using MultiShop.Message.Dtos;

namespace MultiShop.Message.Mapping;

public class MessageMappingProfile : Profile
{
    public MessageMappingProfile()
    {
        CreateMap<DataAccess.Entities.Message, ResultMessageDto>().ReverseMap();
        CreateMap<DataAccess.Entities.Message, ResultInboxMessageDto>().ReverseMap();
        CreateMap<DataAccess.Entities.Message, ResultSendboxMessageDto>().ReverseMap();
        CreateMap<DataAccess.Entities.Message, CreateMessageDto>().ReverseMap();
        CreateMap<DataAccess.Entities.Message, UpdateMessageDto>().ReverseMap();
    }
}


