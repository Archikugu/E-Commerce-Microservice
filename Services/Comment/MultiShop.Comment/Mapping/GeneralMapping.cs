using AutoMapper;
using MultiShop.Comment.Dtos;
using MultiShop.Comment.Entities;

namespace MultiShop.Comment.Mapping;

public class GeneralMapping : Profile
{
    public GeneralMapping()
    {
        CreateMap<UserComment, ResultCommentDto>().ReverseMap();
        CreateMap<UserComment, UpdateCommentDto>().ReverseMap();
        CreateMap<UserComment, CreateCommentDto>().ReverseMap();
    }
}
