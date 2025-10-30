using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.UserIdentityServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class InvoiceController : Controller
{
    private readonly IOrderOrderingService _orderOrderingService;
    private readonly IUserIdentityService _userIdentityService;

    public InvoiceController(IOrderOrderingService orderOrderingService, IUserIdentityService userIdentityService)
    {
        _orderOrderingService = orderOrderingService;
        _userIdentityService = userIdentityService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var orders = await _orderOrderingService.GetOrderingListAsync();
        
        // Filter only Delivered orders for invoices
        var deliveredOrders = orders.Where(o => o.Status?.Equals("Delivered", StringComparison.OrdinalIgnoreCase) == true).ToList();
        
        try
        {
            var users = await _userIdentityService.GetAllUsersAsync();
            var map = users?.GroupBy(u => u.Id).ToDictionary(g => g.Key, g => g.First().FullName ?? string.Empty) ?? new Dictionary<string, string>();
            ViewBag.UserMap = map;
        }
        catch { ViewBag.UserMap = new Dictionary<string, string>(); }

        // Server-side pagination
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        var totalItems = deliveredOrders.Count;
        var skip = (pageNumber - 1) * pageSize;
        var paged = deliveredOrders.OrderByDescending(o => o.OrderDate).Skip(skip).Take(pageSize).ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;

        return View(paged);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        // Fetch order details for the invoice
        // This would typically fetch order details and customer info
        // For now, redirect to order detail
        return RedirectToAction("Index", "Order", new { area = "Admin" });
    }
}

