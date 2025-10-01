using MultiShop.WebUI.Dtos.OrderDtos.OrderAddressDtos;

namespace MultiShop.WebUI.Services.OrderServices.OrderAddressServices;

public interface IOrderAddressService
{
    Task<List<ResultOrderAddressDto>> GetAllAsync();
    Task<GetByIdOrderAddressDto> GetByIdAsync(int id);
    Task CreateAsync(CreateOrderAddressDto dto);
    Task UpdateAsync(UpdateOrderAddressDto dto);
    Task DeleteAsync(int id);
}
