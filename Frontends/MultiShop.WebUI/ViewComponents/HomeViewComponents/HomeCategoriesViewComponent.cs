using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents
{
    public class HomeCategoriesViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
