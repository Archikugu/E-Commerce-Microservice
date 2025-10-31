using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.MessageDtos;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.MessageServices;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
public class MessageController : Controller
{
    private readonly IMessageService _messageService;
    private readonly ILoginService _loginService;

    public MessageController(IMessageService messageService, ILoginService loginService)
    {
        _messageService = messageService;
        _loginService = loginService;
    }

    public async Task<IActionResult> Inbox(int pageNumber = 1, int pageSize = 10, string? q = null)
    {
        var userId = _loginService.GetUserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        var messages = await _messageService.GetInboxAsync(userId);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var qq = q.ToLowerInvariant();
            messages = messages.Where(m => (m.Subject ?? "").ToLowerInvariant().Contains(qq) || (m.MessageDetail ?? "").ToLowerInvariant().Contains(qq)).ToList();
        }
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;
        var ordered = messages.OrderByDescending(m => m.MessageDate).ToList();
        var totalItems = ordered.Count;
        var skip = (pageNumber - 1) * pageSize;
        var paged = ordered.Skip(skip).Take(pageSize).ToList();
        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.Query = q ?? "";
        return View(paged);
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _messageService.MarkAsReadAsync(id);
        return RedirectToAction("Inbox");
    }

    [HttpPost]
    public async Task<IActionResult> MarkAsReadAjax(int id)
    {
        await _messageService.MarkAsReadAsync(id);
        return Ok();
    }

    public async Task<IActionResult> Sendbox(int pageNumber = 1, int pageSize = 10, string? q = null)
    {
        var userId = _loginService.GetUserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        var messages = await _messageService.GetSendboxAsync(userId);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var qq = q.ToLowerInvariant();
            messages = messages.Where(m => (m.Subject ?? "").ToLowerInvariant().Contains(qq) || (m.MessageDetail ?? "").ToLowerInvariant().Contains(qq)).ToList();
        }
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize < 1 ? 10 : pageSize;
        var ordered = messages.OrderByDescending(m => m.MessageDate).ToList();
        var totalItems = ordered.Count;
        var skip = (pageNumber - 1) * pageSize;
        var paged = ordered.Skip(skip).Take(pageSize).ToList();
        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.Query = q ?? "";
        return View(paged);
    }
}
