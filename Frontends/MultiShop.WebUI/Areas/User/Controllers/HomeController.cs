using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.MessageServices;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
public class HomeController : Controller
{
    private readonly IOrderOrderingService _orderService;
    private readonly IMessageService _messageService;
    private readonly IUserService _userService;
    private readonly IProductService _productService;

    public HomeController(IOrderOrderingService orderService, IMessageService messageService, IUserService userService, IProductService productService)
    {
        _orderService = orderService;
        _messageService = messageService;
        _userService = userService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userService.GetUserDetails();
        var orders = await _orderService.GetOrderingByUserId(user.Id);
        var unreadMessages = await _messageService.GetInboxAsync(user.Id);
        int unreadCount = unreadMessages.Count(m => !m.IsRead);

        // wishlist count from session
        int wishlistCount = 0;
        var wishlistNames = new List<string>();
        try
        {
            var json = HttpContext?.Session?.GetString("user_wishlist_ids");
            if (!string.IsNullOrWhiteSpace(json))
            {
                var ids = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
                wishlistCount = ids.Count;

                // Fetch up to 5 product names
                foreach (var pid in ids.Take(5))
                {
                    try
                    {
                        var p = await _productService.GetByIdProductAsync(pid);
                        if (p != null && !string.IsNullOrWhiteSpace(p.ProductName))
                        {
                            wishlistNames.Add(p.ProductName);
                        }
                    }
                    catch { /* ignore per-item failures */ }
                }
            }
        }
        catch { wishlistCount = 0; }

        ViewBag.TotalOrders = orders.Count;
        ViewBag.DeliveredOrders = orders.Count(o => string.Equals(o.Status, "Delivered", StringComparison.OrdinalIgnoreCase));
        ViewBag.ProcessingOrders = orders.Count(o => string.Equals(o.Status, "Processing", StringComparison.OrdinalIgnoreCase));
        ViewBag.UnreadMessages = unreadCount;
        ViewBag.WishlistCount = wishlistCount;
        ViewBag.WishlistNames = wishlistNames;
        ViewBag.RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList();
        ViewBag.OrdersForCharts = orders.OrderBy(o => o.OrderDate).ToList();
        return View();
    }
}
