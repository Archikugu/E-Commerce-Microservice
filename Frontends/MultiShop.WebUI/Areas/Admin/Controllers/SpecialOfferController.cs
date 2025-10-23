using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.SpecialOfferDtos;
using MultiShop.WebUI.Services.CatalogServices.SpecialOfferServices;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class SpecialOfferController : Controller
{
    private readonly ISpecialOfferService _specialOfferService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceAPISettings _apiSettings;
    private readonly IClientCrendentialTokenService _clientTokenService;

    public SpecialOfferController(ISpecialOfferService specialOfferService, IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
    {
        _specialOfferService = specialOfferService;
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiOptions.Value;
        _clientTokenService = clientTokenService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Special Offer";
        ViewBag.v3 = "Special Offer List";

        var values = await _specialOfferService.GetAllSpecialOfferAsync();
        return View(values);
    }

    [HttpGet]
    public async Task<IActionResult> CreateSpecialOffer()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpecialOffer(CreateSpecialOfferDto createSpecialOfferDto)
    {
        await _specialOfferService.CreateSpecialOfferAsync(createSpecialOfferDto);
        return RedirectToAction("Index", "SpecialOffer", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateSpecialOffer(string id)
    {
        var value = await _specialOfferService.GetByIdSpecialOfferAsync(id);
        var model = new UpdateSpecialOfferDto
        {
            SpecialOfferId = value.SpecialOfferId,
            Title = value.Title,
            SubTitle = value.SubTitle,
            ImageUrl = value.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateSpecialOffer(UpdateSpecialOfferDto updateSpecialOfferDto)
    {
        await _specialOfferService.UpdateSpecialOfferAsync(updateSpecialOfferDto);
        return RedirectToAction("Index", "SpecialOffer", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteSpecialOffer(string id)
    {
        await _specialOfferService.DeleteSpecialOfferAsync(id);
        return RedirectToAction("Index", "SpecialOffer", new { area = "Admin" });
    }

    [HttpPost]
    [Area("Admin")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { error = "File is required" });
        }

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
        var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Images.Path.TrimStart('/').TrimEnd('/');

        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        var sc = new StreamContent(stream);
        sc.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content.Add(sc, "file", file.FileName);
        content.Add(new StringContent(file.FileName), "fileName");

        var url1 = baseUrl + "/api/GoogleDriveImageUploads/upload";
        var url2 = baseUrl.TrimEnd('/') + "/GoogleDriveImageUploads/upload";

        var resp = await client.PostAsync(url1, content);
        var body = await resp.Content.ReadAsStringAsync();
        if (resp.IsSuccessStatusCode)
        {
            return Content(body, "application/json");
        }

        using var content2 = new MultipartFormDataContent();
        using var stream2 = file.OpenReadStream();
        var sc2 = new StreamContent(stream2);
        sc2.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content2.Add(sc2, "file", file.FileName);
        content2.Add(new StringContent(file.FileName), "fileName");
        var resp2 = await client.PostAsync(url2, content2);
        var body2 = await resp2.Content.ReadAsStringAsync();
        if (resp2.IsSuccessStatusCode)
        {
            return Content(body2, "application/json");
        }

        return StatusCode((int)resp2.StatusCode, new { error = string.IsNullOrWhiteSpace(body2) ? body : body2 });
    }

    [HttpGet]
    [Area("Admin")]
    public async Task<IActionResult> GetSignedUrl(string fileName, int minutes = 10080)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest(new { error = "fileName is required" });
        }

        if (minutes <= 0) minutes = 30;
        if (minutes > 10080) minutes = 10080;

        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
        var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Images.Path.TrimStart('/').TrimEnd('/');
        var url1 = $"{baseUrl}/api/GoogleDriveImageUploads/signed-url/{Uri.EscapeDataString(fileName)}?minutes={minutes}";
        var resp = await client.GetAsync(url1);
        if (resp.IsSuccessStatusCode)
        {
            var ok = await resp.Content.ReadAsStringAsync();
            return Content(ok, "application/json");
        }
        var url2 = $"{baseUrl}/GoogleDriveImageUploads/signed-url/{Uri.EscapeDataString(fileName)}?minutes={minutes}";
        var resp2 = await client.GetAsync(url2);
        var body2 = await resp2.Content.ReadAsStringAsync();
        if (resp2.IsSuccessStatusCode)
        {
            return Content(body2, "application/json");
        }
        return StatusCode((int)resp2.StatusCode, new { error = string.IsNullOrWhiteSpace(body2) ? $"Failed to get signed url. Tried {url1} and {url2}." : body2 });
    }
}
