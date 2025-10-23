using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using Newtonsoft.Json;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly ServiceAPISettings _apiSettings;
    private readonly IClientCrendentialTokenService _clientTokenService;
    public CategoryController(IHttpClientFactory httpClientFactory, ICategoryService categoryService, IProductService productService, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
    {
        _httpClientFactory = httpClientFactory;
        _categoryService = categoryService;
        _productService = productService;
        _apiSettings = apiOptions.Value;
        _clientTokenService = clientTokenService;
    }
    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Categories";
        ViewBag.v3 = "Category List";

        var all = await _categoryService.GetAllCategoryAsync();
        var totalItems = all.Count;
        var page = Math.Max(1, pageNumber);
        var size = Math.Max(1, pageSize);
        var items = all.Skip((page - 1) * size).Take(size).ToList();

        ViewBag.PageNumber = page;
        ViewBag.PageSize = size;
        ViewBag.TotalItems = totalItems;
        return View(items);
    }
    [HttpGet]
    public async Task<IActionResult> CreateCategory()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto createCategoryDto)
    {
        await _categoryService.CreateCategoryAsync(createCategoryDto);

        return RedirectToAction("Index", "Category", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateCategory(string id)
    {
        var read = await _categoryService.GetByIdCategoryAsync(id);
        var model = new UpdateCategoryDto
        {
            CategoryId = read.CategoryId,
            CategoryName = read.CategoryName,
            ImageUrl = read.ImageUrl
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto updateCategoryDto)
    {
       await _categoryService.UpdateCategoryAsync(updateCategoryDto);
        return RedirectToAction("Index", "Category", new { area = "Admin" });
     
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCategory(string id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return RedirectToAction("Index", "Category", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> ViewProducts(string id, int pageNumber = 1, int pageSize = 10)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Categories";
        ViewBag.v3 = "Products in Category";
        // Kategori adını al
        var category = await _categoryService.GetByIdCategoryAsync(id);
        ViewBag.CategoryName = category.CategoryName;
        ViewBag.CategoryId = id;

        // Kategorideki ürünleri getir (Ocelot + token üzerinden)
        var values = await _productService.GetProductsWithCategoryByCategoryIdAsync(id);

        // Local paging
        var totalItems = values?.Count ?? 0;
        var page = Math.Max(1, pageNumber);
        var size = Math.Max(1, pageSize);
        var items = (values ?? new List<ResultProductsWithCategoryDto>()).Skip((page - 1) * size).Take(size).ToList();

        ViewBag.PageNumber = page;
        ViewBag.PageSize = size;
        ViewBag.TotalItems = totalItems;
        return View(items);
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
        var streamContent = new StreamContent(stream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content.Add(streamContent, "file", file.FileName);
        content.Add(new StringContent(file.FileName), "fileName");
        var url1 = baseUrl + "/api/GoogleDriveImageUploads/upload";
        var url2 = baseUrl.TrimEnd('/') + "/GoogleDriveImageUploads/upload";
        HttpResponseMessage? resp = null;
        string? body = null;
        try
        {
            resp = await client.PostAsync(url1, content);
            body = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return Content(body, "application/json");
            }
        }
        catch (Exception ex)
        {
            body = ex.Message;
        }

        // Try fallback url pattern
        using var content2 = new MultipartFormDataContent();
        using var stream2 = file.OpenReadStream();
        var streamContent2 = new StreamContent(stream2);
        streamContent2.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
        content2.Add(streamContent2, "file", file.FileName);
        content2.Add(new StringContent(file.FileName), "fileName");
        var resp2 = await client.PostAsync(url2, content2);
        var body2 = await resp2.Content.ReadAsStringAsync();
        if (resp2.IsSuccessStatusCode)
        {
            return Content(body2, "application/json");
        }

        var status1 = resp != null ? (int)resp.StatusCode : 0;
        var status2 = (int)resp2.StatusCode;
        return StatusCode(status2 != 0 ? status2 : 500, new { error = string.IsNullOrWhiteSpace(body2) ? (string.IsNullOrWhiteSpace(body) ? $"Upload failed. Tried {url1} (status {status1}) and {url2} (status {status2})." : body) : body2 });
    }
}
