using System.Text;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.FeatureSliderDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.Areas.Admin.Controllers;
[Area("Admin")]
public class FeatureSliderController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public FeatureSliderController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Feature Slider Images";
        ViewBag.v3 = "Feature Slider List";

        var client = _httpClientFactory.CreateClient();
        //Catalog Url 7001
        var responseMessage = await client.GetAsync("https://localhost:7001/api/FeatureSliders");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultFeatureSliderDto>>(jsonData);
            return View(values);
        }

        return View();
    }
    [HttpGet]
    public async Task<IActionResult> CreateFeatureSlider()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateFeatureSlider(CreateFeatureSliderDto createFeatureSliderDto)
    {
        var client = _httpClientFactory.CreateClient();
        var jsonData = JsonConvert.SerializeObject(createFeatureSliderDto);
        StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var responseMessage = await client.PostAsync("https://localhost:7001/api/FeatureSliders", stringContent);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> UpdateFeatureSlider(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.GetAsync("https://localhost:7001/api/FeatureSliders/" + id);
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<UpdateFeatureSliderDto>(jsonData);
            return View(values);
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFeatureSlider(UpdateFeatureSliderDto updateFeatureSliderDto)
    {
        var client = _httpClientFactory.CreateClient();
        var jsonData = JsonConvert.SerializeObject(updateFeatureSliderDto);
        StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
        var responseMessage = await client.PutAsync("https://localhost:7001/api/FeatureSliders", stringContent);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> DeleteFeatureSlider(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.DeleteAsync("https://localhost:7001/api/FeatureSliders/" + id);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ChangeStatusTrue(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.PutAsync("https://localhost:7001/api/FeatureSliders/ChangeStatusTrue/"+id,null);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ChangeStatusFalse(string id)
    {
        var client = _httpClientFactory.CreateClient();
        var responseMessage = await client.PutAsync("https://localhost:7001/api/FeatureSliders/ChangeStatusFalse/"+id, null);
        if (responseMessage.IsSuccessStatusCode)
        {
            return RedirectToAction("Index", "FeatureSlider", new { area = "Admin" });
        }
        return View();
    }
}
