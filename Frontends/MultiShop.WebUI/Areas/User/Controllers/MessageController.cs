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

    public async Task<IActionResult> Inbox()
    {
        var userId = _loginService.GetUserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        var messages = await _messageService.GetInboxAsync(userId);
        return View(messages);
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

    public async Task<IActionResult> Sendbox()
    {
        var userId = _loginService.GetUserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        var messages = await _messageService.GetSendboxAsync(userId);
        return View(messages);
    }
}
