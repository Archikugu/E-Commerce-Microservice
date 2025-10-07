using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.Areas.User.ViewComponents.UserLayoutViewComponents;

public class UserLayoutSidebarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}


