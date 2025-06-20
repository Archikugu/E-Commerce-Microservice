using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeCarouselViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
