using MultiShop.WebUI.Dtos.CatalogDtos.FeatureDtos;

namespace MultiShop.WebUI.Services.CatalogServices.FeatureServices
{
    public interface IFeatureService
    {
        Task<List<ResultFeatureDto>> GetAllAsync();
        Task<GetByIdFeatureDto> GetByIdAsync(string id);
        Task CreateAsync(CreateFeatureDto dto);
        Task UpdateAsync(UpdateFeatureDto dto);
        Task DeleteAsync(string id);
    }
}
