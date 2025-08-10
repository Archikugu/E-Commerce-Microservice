using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ContactDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class ContactController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ContactController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Contacts";
        ViewBag.v3 = "Contact List";

        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.GetAsync("https://localhost:7001/api/Contacts");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultContactDto>>(jsonData);
            return View(values);
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdContact(string id)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Contacts";
        ViewBag.v3 = "Contact Detail";

        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.GetAsync($"https://localhost:7001/api/Contacts/{id}");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<ResultContactDto>(jsonData);
            return View(values);
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UpdateContact(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.GetAsync($"https://localhost:7001/api/Contacts/{id}");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<UpdateContactDto>(jsonData);
            return View(values);
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateContact(UpdateContactDto updateContactDto)
    {
        var client = _httpClientFactory.CreateClient();
        var jsonData = JsonConvert.SerializeObject(updateContactDto);
        StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var responseMessage = await client.PutAsync("https://localhost:7001/api/Contacts", stringContent);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Contact", new { area = "Admin" });
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DeleteContact(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.DeleteAsync($"https://localhost:7001/api/Contacts/{id}");
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "Contact", new { area = "Admin" });
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> MarkAsRead(string id)
    {
        var client = _httpClientFactory.CreateClient();
        
        // Önce contact'ı getir
        var getResponse = await client.GetAsync($"https://localhost:7001/api/Contacts/{id}");
        if (getResponse.IsSuccessStatusCode)
        {
            var jsonData = await getResponse.Content.ReadAsStringAsync();
            var contact = JsonConvert.DeserializeObject<UpdateContactDto>(jsonData);
            
            // IsRead'i true yap
            contact.IsRead = true;
            
            // Güncelle
            var updateJsonData = JsonConvert.SerializeObject(contact);
            StringContent stringContent = new StringContent(updateJsonData, Encoding.UTF8, "application/json");
            var updateResponse = await client.PutAsync("https://localhost:7001/api/Contacts", stringContent);
            
            if (updateResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Contact", new { area = "Admin" });
            }
        }
        
        return RedirectToAction("Index", "Contact", new { area = "Admin" });
    }
}
