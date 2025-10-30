using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Services.UserIdentityServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderController : Controller
{
    private readonly IOrderOrderingService _orderOrderingService;
    private readonly IUserIdentityService _userIdentityService;
    public OrderController(IOrderOrderingService orderOrderingService, IUserIdentityService userIdentityService)
    {
        _orderOrderingService = orderOrderingService;
        _userIdentityService = userIdentityService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        var orders = await _orderOrderingService.GetOrderingListAsync();
        try
        {
            var users = await _userIdentityService.GetAllUsersAsync();
            var map = users?.GroupBy(u => u.Id).ToDictionary(g => g.Key, g => g.First().FullName ?? string.Empty) ?? new Dictionary<string, string>();
            ViewBag.UserMap = map;
        }
        catch { ViewBag.UserMap = new Dictionary<string, string>(); }
        var ordered = orders.OrderByDescending(o => o.OrderDate).ToList();
        // server-side pagination (in-memory)
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;
        var totalItems = ordered.Count;
        var skip = (pageNumber - 1) * pageSize;
        var paged = ordered.Skip(skip).Take(pageSize).ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;

        return View(paged);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        try
        {
            await _orderOrderingService.UpdateStatusAsync(id, status);
            return Ok(new { ok = true });
        }
        catch (Exception ex)
        {
            return BadRequest(new { ok = false, message = ex.Message });
        }
    }
}


