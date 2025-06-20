using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeFeaturedViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
