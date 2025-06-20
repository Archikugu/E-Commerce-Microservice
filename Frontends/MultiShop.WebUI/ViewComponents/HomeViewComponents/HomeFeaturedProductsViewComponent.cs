using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeFeaturedProductsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
