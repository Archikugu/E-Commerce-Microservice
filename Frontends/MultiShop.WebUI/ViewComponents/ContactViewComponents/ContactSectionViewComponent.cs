using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ContactDtos;

namespace MultiShop.WebUI.ViewComponents.ContactViewComponents
{
    public class ContactSectionViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var createContactDto = new CreateContactDto();
            return View(createContactDto);
        }
    }
}
