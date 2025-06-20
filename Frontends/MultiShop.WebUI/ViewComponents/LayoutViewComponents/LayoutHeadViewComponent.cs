using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents
{
    public class LayoutHeadViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
