using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.StatisticsServices.CommentStatisticServices;
using MultiShop.WebUI.Services.MessageServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
[Route("Admin/[controller]")]
public class NotificationController : Controller
{
    private readonly IOrderOrderingService _orderService;
    private readonly ICommentStatisticsService _commentService;
    private readonly IMessageService _messageService;

    public NotificationController(
        IOrderOrderingService orderService,
        ICommentStatisticsService commentService,
        IMessageService messageService)
    {
        _orderService = orderService;
        _commentService = commentService;
        _messageService = messageService;
    }

    [HttpGet]
    [Route("")]
    [Route("Index")]
    public async Task<IActionResult> Index()
    {
        // Get pending orders
        var orders = await _orderService.GetOrderingListAsync();
        var pendingOrders = orders.Where(o => o.Status == "New" || o.Status == "Processing").ToList();
        
        // Get unread messages
        var recentMessages = await _messageService.GetLatestAsync(10);
        var unreadMessages = recentMessages.Where(m => !m.IsRead).ToList();
        
        ViewBag.PendingOrders = pendingOrders.Count;
        ViewBag.UnreadMessages = unreadMessages.Count;
        ViewBag.RecentNotifications = new List<object>();
        
        // Recent notifications list
        foreach (var order in pendingOrders.Take(5))
        {
            ViewBag.RecentNotifications.Add(new { 
                type = "order", 
                title = "New Order", 
                message = $"Order #{order.OrderingId} - ${order.TotalPrice:C}", 
                date = order.OrderDate 
            });
        }
        
        foreach (var msg in unreadMessages.Take(5))
        {
            ViewBag.RecentNotifications.Add(new { 
                type = "message", 
                title = "New Message", 
                message = msg.Subject ?? "(No Subject)", 
                date = msg.MessageDate 
            });
        }

        return View();
    }

    [HttpGet("api/count")]
    public async Task<IActionResult> GetNotificationCount()
    {
        var orders = await _orderService.GetOrderingListAsync();
        var pendingOrders = orders.Where(o => o.Status == "New" || o.Status == "Processing").Count();
        
        var recentMessages = await _messageService.GetLatestAsync(100);
        var unreadMessages = recentMessages.Where(m => !m.IsRead).Count();
        
        var totalCount = pendingOrders + unreadMessages;
        
        return Json(new { count = totalCount, pendingOrders, unreadMessages });
    }
}

