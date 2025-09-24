using MultiShop.WebUI.Dtos.CatalogDtos.OfferDiscountsDtos;

namespace MultiShop.WebUI.Services.CatalogServices.OfferDiscountServices
{
    public interface IOfferDiscountService
    {
        Task<List<ResultOfferDiscountDto>> GetAllAsync();
        Task<GetByIdOfferDiscountDto> GetByIdAsync(string id);
        Task CreateAsync(CreateOfferDiscountDto dto);
        Task UpdateAsync(UpdateOfferDiscountDto dto);
        Task DeleteAsync(string id);
    }
}
