using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.StatisticsServices.CatalogStatistic;
using MultiShop.WebUI.Services.UserIdentityServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CashRegisterController : Controller
{
    private readonly IOrderOrderingService _orderService;
    private readonly ICatalogStatisticsService _catalogService;
    private readonly IUserIdentityService _userService;

    public CashRegisterController(
        IOrderOrderingService orderService,
        ICatalogStatisticsService catalogService,
        IUserIdentityService userService)
    {
        _orderService = orderService;
        _catalogService = catalogService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Load today's statistics
        var orders = await _orderService.GetOrderingListAsync();
        var today = DateTime.UtcNow.Date;
        var todayOrders = orders.Where(o => o.OrderDate.Date == today).ToList();

        var todayRevenue = todayOrders.Sum(o => o.TotalPrice);
        var todayOrderCount = todayOrders.Count;
        var todayDeliveredCount = todayOrders.Count(o => o.Status == "Delivered");

        ViewBag.TodayRevenue = todayRevenue;
        ViewBag.TodayOrderCount = todayOrderCount;
        ViewBag.TodayDeliveredCount = todayDeliveredCount;
        ViewBag.TodayOrders = todayOrders.OrderByDescending(o => o.OrderDate).Take(20).ToList();

        try
        {
            var users = await _userService.GetAllUsersAsync();
            var map = users?.GroupBy(u => u.Id).ToDictionary(g => g.Key, g => g.First().FullName ?? string.Empty) ?? new Dictionary<string, string>();
            ViewBag.UserMap = map;
        }
        catch
        {
            ViewBag.UserMap = new Dictionary<string, string>();
        }

        return View();
    }
}

