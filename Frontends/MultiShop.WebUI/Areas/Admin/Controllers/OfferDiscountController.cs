using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.OfferDiscountsDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.Areas.Admin.Controllers;
[Area("Admin")]
[AllowAnonymous]
public class OfferDiscountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OfferDiscountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Offer Discount";
        ViewBag.v3 = "Offer Discount List";

        var client = _httpClientFactory.CreateClient();
        //Catalog Url 7001
        var responseMessage = await client.GetAsync("https://localhost:7001/api/OfferDiscounts");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultOfferDiscountDto>>(jsonData);
            return View(values);
        }

        return View();
    }
    [HttpGet]
    public async Task<IActionResult> CreateOfferDiscount()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateOfferDiscount(CreateOfferDiscountDto createOfferDiscountDto)
    {
        var client = _httpClientFactory.CreateClient();
        var jsonData = JsonConvert.SerializeObject(createOfferDiscountDto);
        StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var responseMessage = await client.PostAsync("https://localhost:7001/api/OfferDiscounts", stringContent);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UpdateOfferDiscount(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.GetAsync("https://localhost:7001/api/OfferDiscounts/" + id);
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<UpdateOfferDiscountDto>(jsonData);
            return View(values);
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOfferDiscount(UpdateOfferDiscountDto updateOfferDiscountDto)
    {
        var client = _httpClientFactory.CreateClient();
        var jsonData = JsonConvert.SerializeObject(updateOfferDiscountDto);
        StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var responseMessage = await client.PutAsync("https://localhost:7001/api/OfferDiscounts", stringContent);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DeleteOfferDiscount(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.DeleteAsync("https://localhost:7001/api/OfferDiscounts/" + id);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "OfferDiscount", new { area = "Admin" });
        }
        return View();
    }
}
