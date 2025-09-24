using MultiShop.WebUI.Dtos.CatalogDtos.FeatureSliderDtos;

namespace MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices
{
    public interface IFeatureSliderService
    {
        Task<List<ResultFeatureSliderDto>> GetAllAsync();
        Task<ResultFeatureSliderDto> GetByIdAsync(string id);
        Task CreateAsync(CreateFeatureSliderDto dto);
        Task UpdateAsync(UpdateFeatureSliderDto dto);
        Task DeleteAsync(string id);
        Task ChangeStatusTrueAsync(string id);
        Task ChangeStatusFalseAsync(string id);
        Task ToggleAsync(string id);
    }
}
