using MultiShop.WebUI.Dtos.CatalogDtos.AboutDtos;

namespace MultiShop.WebUI.Services.CatalogServices.AboutServices
{
    public interface IAboutService
    {
        Task<List<ResultAboutDto>> GetAllAsync();
        Task<ResultAboutDto> GetByIdAsync(string id);
        Task CreateAsync(CreateAboutDto dto);
        Task UpdateAsync(UpdateAboutDto dto);
        Task DeleteAsync(string id);
    }
}
