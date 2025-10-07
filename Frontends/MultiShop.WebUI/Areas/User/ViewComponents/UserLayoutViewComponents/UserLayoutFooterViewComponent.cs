using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.Areas.User.ViewComponents.UserLayoutViewComponents
{
    public class UserLayoutFooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
