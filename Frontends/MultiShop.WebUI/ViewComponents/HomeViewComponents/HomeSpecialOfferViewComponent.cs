using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeSpecialOfferViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
