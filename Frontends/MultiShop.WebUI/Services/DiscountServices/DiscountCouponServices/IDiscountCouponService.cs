using MultiShop.WebUI.Dtos.DiscountDtos.DiscountCouponDtos;

namespace MultiShop.WebUI.Services.DiscountServices.DiscountCouponServices
{
    public interface IDiscountCouponService
    {
        Task<List<ResultDiscountCouponDto>> GetAllAsync();
        Task<GetByIdDiscountCouponDto> GetByIdAsync(int id);
        Task<GetByIdDiscountCouponDto> GetByCodeAsync(string code);
        Task CreateAsync(CreateDiscountCouponDto dto);
        Task UpdateAsync(UpdateDiscountCouponDto dto);
        Task DeleteAsync(int id);
    }
}


