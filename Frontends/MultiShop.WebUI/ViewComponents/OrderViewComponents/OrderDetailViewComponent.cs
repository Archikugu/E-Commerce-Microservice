using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.OrderDtos.OrderAddressDtos;

namespace MultiShop.WebUI.ViewComponents.OrderViewComponents;

public class OrderDetailViewComponent :ViewComponent
{
    public IViewComponentResult Invoke(CreateOrderAddressDto model)
    {
        return View(model);
    }
}
