using MultiShop.Discount.Dtos.DiscountCouponDtos;

namespace MultiShop.Discount.Services.DiscountServices;

public interface IDiscountService
{
    Task<List<ResultDiscountCouponDto>> GetAllDiscountCouponAsync();
    Task CreateDiscountCouponAsync(CreateDiscountCouponDto createDiscountCouponDto);
    Task UpdateDiscountCouponAsync(UpdateDiscountCouponDto updateDiscountCouponDto);
    Task DeleteDiscountCouponAsync(int id);
    Task<GetByIdDiscountCouponDto> GetByIdDiscountCouponAsync(int id);
}
