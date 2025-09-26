using MultiShop.WebUI.Dtos.CatalogDtos.ContactDtos;

namespace MultiShop.WebUI.Services.CatalogServices.ContactServices
{
    public interface IContactService
    {
        Task<List<ResultContactDto>> GetAllAsync();
        Task<ResultContactDto> GetByIdAsync(string id);
        Task CreateAsync(CreateContactDto dto);
        Task UpdateAsync(UpdateContactDto dto);
        Task DeleteAsync(string id);
    }
}
