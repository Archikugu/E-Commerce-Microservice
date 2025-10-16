using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.MessageServices;
using MultiShop.WebUI.Services.UserIdentityServices;

namespace MultiShop.WebUI.Areas.Admin.ViewComponents.AdminLayoutViewComponents
{
    public class AdminLayoutHeaderViewComponent : ViewComponent
    {
        private readonly IMessageService _messageService;
        private readonly ILoginService _loginService;
        private readonly IUserIdentityService _userIdentityService;

        public AdminLayoutHeaderViewComponent(IMessageService messageService, ILoginService loginService, IUserIdentityService userIdentityService)
        {
            _messageService = messageService;
            _loginService = loginService;
            _userIdentityService = userIdentityService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _loginService.GetUserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                ViewBag.MessageCount = 0;
                return View(new List<MultiShop.WebUI.Dtos.MessageDtos.ResultInboxMessageDto>());
            }

            var inbox = await _messageService.GetInboxAsync(userId);
            ViewBag.MessageCount = inbox?.Count ?? 0;
            // Dropdown’u çok doldurmamak için ilk 5 mesajı gösterelim
            var top = (inbox ?? new List<MultiShop.WebUI.Dtos.MessageDtos.ResultInboxMessageDto>()).Take(5).ToList();

            // SenderName doldurma (IdentityServer’dan)
            try
            {
                var users = await _userIdentityService.GetAllUsersAsync();
                var idToName = users?.ToDictionary(u => u.Id, u => string.IsNullOrWhiteSpace(u.FullName) ? (u.UserName ?? u.Id) : u.FullName)
                               ?? new Dictionary<string, string>();
                foreach (var m in top)
                {
                    if (!string.IsNullOrWhiteSpace(m.SenderId) && idToName.TryGetValue(m.SenderId!, out var name))
                    {
                        m.SenderName = name;
                    }
                    else if (string.IsNullOrWhiteSpace(m.SenderName) && !string.IsNullOrWhiteSpace(m.SenderId))
                    {
                        m.SenderName = m.SenderId; // fallback
                    }
                }
            }
            catch
            {
                foreach (var m in top)
                {
                    if (string.IsNullOrWhiteSpace(m.SenderName) && !string.IsNullOrWhiteSpace(m.SenderId))
                    {
                        m.SenderName = m.SenderId;
                    }
                }
            }

            return View(top);
        }
    }
}
