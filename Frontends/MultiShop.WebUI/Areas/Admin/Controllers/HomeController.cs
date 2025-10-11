using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;
using MultiShop.WebUI.Services.StatisticsServices.UserStatistic;
using MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;
using MultiShop.WebUI.Services.MessageServices;
using MultiShop.WebUI.Services.StatisticsServices.DiscountStatisticServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ICatalogStatisticsService _statisticsService;
        private readonly IUserStatisticsService _userStatisticsService;
        private readonly ICommentStatisticsService _commentStatisticsService;
        private readonly IMessageService _messageService;
        private readonly IDiscountStatisticsService _discountStatisticsService;

        public HomeController(ICatalogStatisticsService statisticsService, IUserStatisticsService userStatisticsService, ICommentStatisticsService commentStatisticsService, IMessageService messageService, IDiscountStatisticsService discountStatisticsService)
        {
            _statisticsService = statisticsService;
            _userStatisticsService = userStatisticsService;
            _commentStatisticsService = commentStatisticsService;
            _messageService = messageService;
            _discountStatisticsService = discountStatisticsService;
        }

        public async Task<IActionResult> Index()
        {
            var categoryCount = await _statisticsService.GetCategoryCountAsync();
            var productCount = await _statisticsService.GetProductCountAsync();
            var brandCount = await _statisticsService.GetBrandCountAsync();
            var averagePrice = await _statisticsService.GetAverageProductPriceAsync();
            var userCount = await _userStatisticsService.GetUserCountAsync();
            var cheapest = await _statisticsService.GetCheapestProductAsync();
            var mostExpensive = await _statisticsService.GetMostExpensiveProductAsync();
            var activeComments = await _commentStatisticsService.GetActiveCommentCountAsync();
            var passiveComments = await _commentStatisticsService.GetPassiveCommentCountAsync();
            var totalComments = await _commentStatisticsService.GetTotalCommentCountAsync();

            ViewBag.CategoryCount = categoryCount;
            ViewBag.ProductCount = productCount;
            ViewBag.BrandCount = brandCount;
            ViewBag.AverageProductPrice = averagePrice;
            ViewBag.CheapestProduct = cheapest;
            ViewBag.MostExpensiveProduct = mostExpensive;
            ViewBag.UserCount = userCount;
            ViewBag.ActiveCommentCount = activeComments;
            ViewBag.PassiveCommentCount = passiveComments;
            ViewBag.TotalCommentCount = totalComments;

            var recentMessages = await _messageService.GetLatestAsync(3);
            ViewBag.RecentMessages = recentMessages;

            // Discount statistics
            var totalCoupons = await _discountStatisticsService.GetTotalDiscountCouponCountAsync();
            var activeCoupons = await _discountStatisticsService.GetActiveDiscountCouponCountAsync();
            var inactiveCoupons = await _discountStatisticsService.GetInactiveDiscountCouponCountAsync();
            var expiredCoupons = await _discountStatisticsService.GetExpiredDiscountCouponCountAsync();
            var validCoupons = await _discountStatisticsService.GetNonExpiredDiscountCouponCountAsync();

            ViewBag.TotalCoupons = totalCoupons;
            ViewBag.ActiveCoupons = activeCoupons;
            ViewBag.InactiveCoupons = inactiveCoupons;
            ViewBag.ExpiredCoupons = expiredCoupons;
            ViewBag.ValidCoupons = validCoupons;

            return View();
        }
    }
}
