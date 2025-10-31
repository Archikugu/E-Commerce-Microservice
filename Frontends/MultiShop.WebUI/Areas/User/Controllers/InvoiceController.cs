using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.OrderServices.OrderDetailServices;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class InvoiceController : Controller
{
    private readonly IOrderDetailService _orderDetailService;
    private readonly IOrderOrderingService _orderOrderingService;

    public InvoiceController(IOrderDetailService orderDetailService, IOrderOrderingService orderOrderingService)
    {
        _orderDetailService = orderDetailService;
        _orderOrderingService = orderOrderingService;
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        if (id <= 0) return RedirectToAction("MyOrderList", "MyOrder");
        var items = await _orderDetailService.GetByOrderingIdAsync(id);
        var orders = await _orderOrderingService.GetOrderingListAsync();
        var order = orders.FirstOrDefault(o => o.OrderingId == id);
        ViewBag.Order = order;
        return View(items);
    }
}


