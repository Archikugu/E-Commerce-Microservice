using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceAPISettings _apiSettings;
    private readonly IClientCrendentialTokenService _clientTokenService;

    public ProductController(IProductService productService, ICategoryService categoryService, IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _httpClientFactory = httpClientFactory;
        _apiSettings = apiOptions.Value;
        _clientTokenService = clientTokenService;
    }

    private async Task PopulateCategoriesAsync(string? selectedCategoryId = null)
    {
        var categories = await _categoryService.GetAllCategoryAsync();
        var items = categories.Select(c => new SelectListItem
        {
            Value = c.CategoryId,
            Text = c.CategoryName,
            Selected = selectedCategoryId != null && c.CategoryId == selectedCategoryId
        }).ToList();
        ViewBag.CategoryList = items;
        ViewBag.CategoryId = items; // bazı view'lar CategoryId anahtarını bekleyebilir
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
    {
        ViewBag.v1 = "Home";
        ViewBag.v2 = "Products";
        ViewBag.v3 = "Product List";

        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var all = await _productService.GetProductsWithCategoryAsync();
        var totalItems = all?.Count ?? 0;
        var items = (all ?? new List<ResultProductsWithCategoryDto>())
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> CreateProduct()
    {
        await PopulateCategoriesAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(createProductDto.CategoryId);
            return View(createProductDto);
        }
        await _productService.CreateProductAsync(createProductDto);
        return RedirectToAction("Index", "Product", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> UpdateProduct(string id)
    {
        var read = await _productService.GetByIdProductAsync(id);
        var model = new UpdateProductDto
        {
            ProductId = read.ProductId,
            ProductName = read.ProductName,
            CategoryId = read.CategoryId,
            Price = read.Price,
            Description = read.Description,
            ImageUrl = read.ImageUrl
        };
        await PopulateCategoriesAsync(model.CategoryId);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProduct(UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(updateProductDto.CategoryId);
            return View(updateProductDto);
        }
        await _productService.UpdateProductAsync(updateProductDto);
        return RedirectToAction("Index", "Product", new { area = "Admin" });
    }

    [HttpGet]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        await _productService.DeleteProductAsync(id);
        return RedirectToAction("Index", "Product", new { area = "Admin" });
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
