using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ContactDtos;
using MultiShop.WebUI.Services.CatalogServices.ContactServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class ContactController : Controller
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Contacts";
        ViewBag.v3 = "Contact List";

        var values = await _contactService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdContact(string id)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Contacts";
        ViewBag.v3 = "Contact Detail";

        var value = await _contactService.GetByIdAsync(id);
        return View(value);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateContact(string id)
    {
        var value = await _contactService.GetByIdAsync(id);
        var model = new UpdateContactDto
        {
            ContactId = value.ContactId,
            Name = value.Name,
            Phone = value.Phone,
            Email = value.Email,
            Subject = value.Subject,
            Message = value.Message,
            IsRead = value.IsRead,
            SendDate = value.SendDate
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateContact(UpdateContactDto updateContactDto)
    {
        await _contactService.UpdateAsync(updateContactDto);
        return RedirectToAction("Index", "Contact", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteContact(string id)
    {
        await _contactService.DeleteAsync(id);
        return RedirectToAction("Index", "Contact", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var value = await _contactService.GetByIdAsync(id);
        var update = new UpdateContactDto
        {
            ContactId = value.ContactId,
            Name = value.Name,
            Phone = value.Phone,
            Email = value.Email,
            Subject = value.Subject,
            Message = value.Message,
            IsRead = true,
            SendDate = value.SendDate
        };
        await _contactService.UpdateAsync(update);
        return RedirectToAction("Index", "Contact", new { area = "Admin" });
    }
}
