using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;
using MultiShop.WebUI.Services.StatisticsServices.UserStatistic;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;
using MultiShop.WebUI.Services.StatisticsServices.DiscountStatisticServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SettingController : Controller
{
    private readonly ICatalogStatisticsService _catalogStats;
    private readonly IUserStatisticsService _userStats;
    private readonly ICommentStatisticsService _commentStats;
    private readonly IOrderOrderingService _orderService;
    private readonly IDiscountStatisticsService _discountStats;

    public SettingController(
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
        // Load current statistics for display
        var orders = await _orderService.GetOrderingListAsync();
        var totalRevenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalPrice);
        var totalOrders = orders.Count;

        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.TotalOrders = totalOrders;
        ViewBag.UserCount = await _userStats.GetUserCountAsync();
        ViewBag.ProductCount = await _catalogStats.GetProductCountAsync();

        return View();
    }

    [HttpPost]
    public IActionResult UpdateGeneralSettings()
    {
        // Placeholder for general settings update
        ViewBag.SuccessMessage = "Settings updated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateEmailSettings()
    {
        // Placeholder for email settings update
        ViewBag.SuccessMessage = "Email settings updated successfully";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdatePaymentSettings()
    {
        // Placeholder for payment settings update
        ViewBag.SuccessMessage = "Payment settings updated successfully";
        return RedirectToAction("Index");
    }
}

