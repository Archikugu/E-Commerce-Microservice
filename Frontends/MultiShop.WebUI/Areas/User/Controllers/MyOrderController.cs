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

    public async Task<IActionResult> MyOrderList()
    {
        var user = await _userService.GetUserDetails();
        var values = await _orderOrderingService.GetOrderingByUserId(user.Id);
        return View(values);
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
