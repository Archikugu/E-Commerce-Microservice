using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;
using MultiShop.WebUI.Services.StatisticsServices.UserStatistic;
using MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.StatisticsServices.DiscountStatisticServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ReportController : Controller
{
    private readonly ICatalogStatisticsService _catalogStats;
    private readonly IUserStatisticsService _userStats;
    private readonly ICommentStatisticsService _commentStats;
    private readonly IOrderOrderingService _orderService;
    private readonly IDiscountStatisticsService _discountStats;

    public ReportController(
        ICatalogStatisticsService catalogStats,
        IUserStatisticsService userStats,
        ICommentStatisticsService commentStats,
        IOrderOrderingService orderService,
        IDiscountStatisticsService discountStats)
    {
        _catalogStats = catalogStats;
        _userStats = userStats;
        _commentStats = commentStats;
        _orderService = orderService;
        _discountStats = discountStats;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Sales Report
        var orders = await _orderService.GetOrderingListAsync();
        var totalRevenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalPrice);
        var totalOrders = orders.Count;
        var deliveredOrders = orders.Count(o => o.Status == "Delivered");
        var pendingOrders = orders.Count(o => o.Status != "Delivered" && o.Status != "Cancelled");

        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.TotalOrders = totalOrders;
        ViewBag.DeliveredOrders = deliveredOrders;
        ViewBag.PendingOrders = pendingOrders;

        // Customer Report
        var userCount = await _userStats.GetUserCountAsync();
        ViewBag.UserCount = userCount;

        // Catalog Report
        var categoryCount = await _catalogStats.GetCategoryCountAsync();
        var productCount = await _catalogStats.GetProductCountAsync();
        var brandCount = await _catalogStats.GetBrandCountAsync();
        var averagePrice = await _catalogStats.GetAverageProductPriceAsync();
        var cheapest = await _catalogStats.GetCheapestProductAsync();
        var mostExpensive = await _catalogStats.GetMostExpensiveProductAsync();

        ViewBag.CategoryCount = categoryCount;
        ViewBag.ProductCount = productCount;
        ViewBag.BrandCount = brandCount;
        ViewBag.AveragePrice = averagePrice;
        ViewBag.CheapestProduct = cheapest;
        ViewBag.MostExpensiveProduct = mostExpensive;

        // Comments
        var activeComments = await _commentStats.GetActiveCommentCountAsync();
        var passiveComments = await _commentStats.GetPassiveCommentCountAsync();
        var totalComments = await _commentStats.GetTotalCommentCountAsync();

        ViewBag.ActiveComments = activeComments;
        ViewBag.PassiveComments = passiveComments;
        ViewBag.TotalComments = totalComments;

        // Discounts
        var totalCoupons = await _discountStats.GetTotalDiscountCouponCountAsync();
        var activeCoupons = await _discountStats.GetActiveDiscountCouponCountAsync();
        var expiredCoupons = await _discountStats.GetExpiredDiscountCouponCountAsync();

        ViewBag.TotalCoupons = totalCoupons;
        ViewBag.ActiveCoupons = activeCoupons;
        ViewBag.ExpiredCoupons = expiredCoupons;

        return View();
    }
}

