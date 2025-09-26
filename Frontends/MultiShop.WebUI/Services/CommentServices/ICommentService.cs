using MultiShop.WebUI.Dtos.CommentDtos;

namespace MultiShop.WebUI.Services.CommentServices
{
    public interface ICommentService
    {
        Task<List<ResultCommentDto>> GetAllAsync();
        Task<List<ResultCommentDto>> GetByProductIdAsync(string productId);
        Task<ResultCommentDto> GetByIdAsync(string id);
        Task CreateAsync(CreateCommentDto dto);
        Task UpdateAsync(UpdateCommentDto dto);
        Task DeleteAsync(string id);
        Task UpdateStatusAsync(string id, bool status);
    }
}
