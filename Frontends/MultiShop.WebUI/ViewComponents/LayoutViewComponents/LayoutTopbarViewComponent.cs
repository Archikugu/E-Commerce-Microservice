using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents
{
    public class LayoutTopbarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
