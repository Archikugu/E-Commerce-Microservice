using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.AboutDtos;
using MultiShop.WebUI.Services.CatalogServices.AboutServices;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class AboutController : Controller
{
    private readonly IAboutService _aboutService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceAPISettings _apiSettings;
    private readonly IClientCrendentialTokenService _clientTokenService;

    public AboutController(IAboutService aboutService, IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
    {
        _aboutService = aboutService;
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiOptions.Value;
        _clientTokenService = clientTokenService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "About";
        ViewBag.v3 = "About List";

        var values = await _aboutService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateAbout()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAbout(CreateAboutDto createAboutDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createAboutDto);
        }
        await _aboutService.CreateAsync(createAboutDto);
        return RedirectToAction("Index", "About", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateAbout(string id)
    {
        var read = await _aboutService.GetByIdAsync(id);
        var model = new UpdateAboutDto
        {
            AboutId = read.AboutId,
            Title = read.Title,
            ImageUrl = read.ImageUrl,
            Description = read.Description,
            Phone = read.Phone,
            Address = read.Address,
            Email = read.Email,
            WorkingHours = read.WorkingHours
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAbout(UpdateAboutDto updateAboutDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateAboutDto);
        }
        await _aboutService.UpdateAsync(updateAboutDto);
        return RedirectToAction("Index", "About", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteAbout(string id)
    {
        await _aboutService.DeleteAsync(id);
        return RedirectToAction("Index", "About", new { area = "Admin" });
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