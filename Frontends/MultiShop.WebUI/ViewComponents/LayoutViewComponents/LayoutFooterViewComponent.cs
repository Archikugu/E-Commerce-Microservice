using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents
{
    public class LayoutFooterViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
