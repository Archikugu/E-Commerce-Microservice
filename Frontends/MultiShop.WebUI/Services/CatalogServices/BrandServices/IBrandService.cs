using MultiShop.WebUI.Dtos.CatalogDtos.BrandDtos;

namespace MultiShop.WebUI.Services.CatalogServices.BrandServices
{
    public interface IBrandService
    {
        Task<List<ResultBrandDto>> GetAllAsync();
        Task<GetByIdBrandDto> GetByIdAsync(string id);
        Task CreateAsync(CreateBrandDto dto);
        Task UpdateAsync(UpdateBrandDto dto);
        Task DeleteAsync(string id);
    }
}
