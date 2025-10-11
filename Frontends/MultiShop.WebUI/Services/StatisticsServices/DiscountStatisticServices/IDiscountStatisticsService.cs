using System.Threading.Tasks;

namespace MultiShop.WebUI.Services.StatisticsServices.DiscountStatisticServices;

public interface IDiscountStatisticsService
{
    Task<int> GetTotalDiscountCouponCountAsync();
    Task<int> GetActiveDiscountCouponCountAsync();
    Task<int> GetInactiveDiscountCouponCountAsync();
    Task<int> GetExpiredDiscountCouponCountAsync();
    Task<int> GetNonExpiredDiscountCouponCountAsync();
}


