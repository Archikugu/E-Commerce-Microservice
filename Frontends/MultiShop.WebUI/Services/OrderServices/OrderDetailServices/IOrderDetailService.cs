using MultiShop.WebUI.Dtos.OrderDtos.OrderDetailDtos;

namespace MultiShop.WebUI.Services.OrderServices.OrderDetailServices;

public interface IOrderDetailService
{
    Task<List<ResultOrderDetailDto>> GetAllAsync();
    Task<List<ResultOrderDetailDto>> GetByOrderingIdAsync(int orderingId);
}


