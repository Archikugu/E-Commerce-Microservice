using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.ContactViewComponents
{
    public class ContactSectionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
