using MultiShop.WebUI.Dtos.OrderDtos.OrderOrderingDtos;

namespace MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;

public interface IOrderOrderingService
{
    Task<List<ResultOrderingByUserIdDto>> GetOrderingByUserId(string userId);
    Task<int> CreateOrderingAsync(CreateOrderingDto dto);
    Task CreateOrderDetailAsync(CreateOrderDetailDto dto);
}
