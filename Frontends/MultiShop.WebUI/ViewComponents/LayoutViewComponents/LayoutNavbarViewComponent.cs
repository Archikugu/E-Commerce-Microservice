using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents
{
    public class LayoutNavbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
