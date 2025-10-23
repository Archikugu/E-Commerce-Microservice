using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.BrandDtos;
using MultiShop.WebUI.Services.CatalogServices.BrandServices;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class BrandController : Controller
{
    private readonly IBrandService _brandService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceAPISettings _apiSettings;
    private readonly IClientCrendentialTokenService _clientTokenService;

    public BrandController(IBrandService brandService, IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
    {
        _brandService = brandService;
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiOptions.Value;
        _clientTokenService = clientTokenService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Brands";
        ViewBag.v3 = "Brand List";

        var all = await _brandService.GetAllAsync();
        var totalItems = all?.Count ?? 0;
        var page = Math.Max(1, pageNumber);
        var size = Math.Max(1, pageSize);
        var items = (all ?? new List<ResultBrandDto>()).Skip((page - 1) * size).Take(size).ToList();

        ViewBag.PageNumber = page;
        ViewBag.PageSize = size;
        ViewBag.TotalItems = totalItems;
        return View(items);
    }

    [HttpGet]
    public IActionResult CreateBrand()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBrand(CreateBrandDto createBrandDto)
    {
        if (!ModelState.IsValid)
        {
            return View(createBrandDto);
        }
        await _brandService.CreateAsync(createBrandDto);
        return RedirectToAction("Index", "Brand", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateBrand(string id)
    {
        var read = await _brandService.GetByIdAsync(id);
        var model = new UpdateBrandDto
        {
            BrandId = read.BrandId,
            BrandName = read.BrandName,
            ImageUrl = read.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBrand(UpdateBrandDto updateBrandDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateBrandDto);
        }
        await _brandService.UpdateAsync(updateBrandDto);
        return RedirectToAction("Index", "Brand", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteBrand(string id)
    {
        await _brandService.DeleteAsync(id);
        return RedirectToAction("Index", "Brand", new { area = "Admin" });
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