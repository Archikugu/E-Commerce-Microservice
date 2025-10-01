using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.ViewComponents.OrderViewComponents;

public class PaymentMethodViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
