using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.OrderServices.OrderDetailServices;
using MultiShop.WebUI.Dtos.OrderDtos.OrderDetailDtos;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
public class MyOrderController : Controller
{
    private readonly IOrderOrderingService _orderOrderingService;
    private readonly IUserService _userService;
    private readonly IOrderDetailService _orderDetailService;

    public MyOrderController(IOrderOrderingService orderOrderingService, IUserService userService, IOrderDetailService orderDetailService)
    {
        _orderOrderingService = orderOrderingService;
        _userService = userService;
        _orderDetailService = orderDetailService;
    }

    public async Task<IActionResult> MyOrderList(int pageNumber = 1, int pageSize = 10, string? status = null, DateTime? from = null, DateTime? to = null)
    {
        var user = await _userService.GetUserDetails();
        var values = await _orderOrderingService.GetOrderingByUserId(user.Id);
        // filters
        if (!string.IsNullOrWhiteSpace(status) && !string.Equals(status, "All", StringComparison.OrdinalIgnoreCase))
        {
            values = values.Where(o => string.Equals(o.Status ?? "", status, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (from.HasValue)
        {
            var f = from.Value.Date;
            values = values.Where(o => o.OrderDate.Date >= f).ToList();
        }
        if (to.HasValue)
        {
            var t = to.Value.Date;
            values = values.Where(o => o.OrderDate.Date <= t).ToList();
        }

        // pagination
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;
        var ordered = values.OrderByDescending(o => o.OrderDate).ToList();
        var totalItems = ordered.Count;
        var skip = (pageNumber - 1) * pageSize;
        var paged = ordered.Skip(skip).Take(pageSize).ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.SelectedStatus = status ?? "All";
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");

        return View(paged);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        if (id <= 0) return RedirectToAction("MyOrderList");
        var items = await _orderDetailService.GetByOrderingIdAsync(id);
        ViewBag.OrderingId = id;
        return View(items);
    }
}
