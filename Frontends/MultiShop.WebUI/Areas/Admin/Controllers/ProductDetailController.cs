using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDetailDtos;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class ProductDetailController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceAPISettings _apiSettings;
        private readonly IClientCrendentialTokenService _clientTokenService;

        public ProductDetailController(IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiOptions.Value;
            _clientTokenService = clientTokenService;
        }

        private string GetCatalogBaseUrl()
        {
            return _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
        }

        private async Task<List<SelectListItem>> FetchProductSelectListAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();
            var items = new List<SelectListItem>();
            var resp = await client.GetAsync(baseUrl + "/api/Products");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var vals = JsonConvert.DeserializeObject<List<ResultProductDto>>(json) ?? new List<ResultProductDto>();
                items = vals.Select(x => new SelectListItem { Text = x.ProductName, Value = x.ProductId }).ToList();
            }
            else
            {
                var fb = await client.GetAsync("https://localhost:7001/api/Products");
                if (fb.IsSuccessStatusCode)
                {
                    var json = await fb.Content.ReadAsStringAsync();
                    var vals = JsonConvert.DeserializeObject<List<ResultProductDto>>(json) ?? new List<ResultProductDto>();
                    items = vals.Select(x => new SelectListItem { Text = x.ProductName, Value = x.ProductId }).ToList();
                }
            }
            return items;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.v1 = "Home";
            ViewBag.v2 = "Product Details";
            ViewBag.v3 = "Product Detail List";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();

            // Product Details (token + fallback)
            var list = new List<ResultProductDetailDto>();
            var resp = await client.GetAsync(baseUrl + "/api/ProductDetails");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<ResultProductDetailDto>>(json) ?? new List<ResultProductDetailDto>();
            }
            else
            {
                var fb = await client.GetAsync("https://localhost:7001/api/ProductDetails");
                if (fb.IsSuccessStatusCode)
                {
                    var json = await fb.Content.ReadAsStringAsync();
                    list = JsonConvert.DeserializeObject<List<ResultProductDetailDto>>(json) ?? new List<ResultProductDetailDto>();
                }
            }

            // Products (for mapping)
            var prodResp = await client.GetAsync(baseUrl + "/api/Products");
            if (prodResp.IsSuccessStatusCode)
            {
                var json = await prodResp.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ResultProductDto>>(json) ?? new List<ResultProductDto>();
                ViewBag.Products = products;
            }
            else
            {
                var fb = await client.GetAsync("https://localhost:7001/api/Products");
                if (fb.IsSuccessStatusCode)
                {
                    var json = await fb.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ResultProductDto>>(json) ?? new List<ResultProductDto>();
                    ViewBag.Products = products;
                }
                else
                {
                    ViewBag.Products = new List<ResultProductDto>();
                }
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProductDetail()
        {
            ViewBag.Products = await FetchProductSelectListAsync();
            return View(new CreateProductDetailDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductDetail(CreateProductDetailDto createProductDetailDto)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();
            var payload = JsonConvert.SerializeObject(createProductDetailDto);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync(baseUrl + "/api/ProductDetails", content);
            if (resp.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductDetail", new { area = "Admin" });
            }
            // Fallback direct
            var fb = await client.PostAsync("https://localhost:7001/api/ProductDetails", content);
            if (fb.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductDetail", new { area = "Admin" });
            }
            var err1 = await resp.Content.ReadAsStringAsync();
            var err2 = await fb.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(err2)) ModelState.AddModelError(string.Empty, err2);
            else if (!string.IsNullOrWhiteSpace(err1)) ModelState.AddModelError(string.Empty, err1);
            ViewBag.Products = await FetchProductSelectListAsync();
            return View(createProductDetailDto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProductDetail(string id)
        {
            ViewBag.Products = await FetchProductSelectListAsync();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();
            var resp = await client.GetAsync(baseUrl + "/api/ProductDetails/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<UpdateProductDetailDto>(json) ?? new UpdateProductDetailDto();
                return View(model);
            }
            else
            {
                var fb = await client.GetAsync("https://localhost:7001/api/ProductDetails/" + id);
                if (fb.IsSuccessStatusCode)
                {
                    var json = await fb.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<UpdateProductDetailDto>(json) ?? new UpdateProductDetailDto();
                    return View(model);
                }
            }
            return View(new UpdateProductDetailDto());
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductDetail(UpdateProductDetailDto updateProductDetailDto)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();
            var payload = JsonConvert.SerializeObject(updateProductDetailDto);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var resp = await client.PutAsync(baseUrl + "/api/ProductDetails", content);
            if (resp.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductDetail", new { area = "Admin" });
            }
            // Fallback direct
            var fb = await client.PutAsync("https://localhost:7001/api/ProductDetails", content);
            if (fb.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductDetail", new { area = "Admin" });
            }
            var err1 = await resp.Content.ReadAsStringAsync();
            var err2 = await fb.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(err2)) ModelState.AddModelError(string.Empty, err2);
            else if (!string.IsNullOrWhiteSpace(err1)) ModelState.AddModelError(string.Empty, err1);
            ViewBag.Products = await FetchProductSelectListAsync();
            return View(updateProductDetailDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetByIdProductDetail(string id)
        {
            ViewBag.v1 = "Home";
            ViewBag.v2 = "Product Details";
            ViewBag.v3 = "Product Detail";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();
            var resp = await client.GetAsync(baseUrl + "/api/ProductDetails/GetById/" + id);
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<GetByIdProductDetailDto>(json) ?? new GetByIdProductDetailDto();
                return View(model);
            }
            else
            {
                var fb = await client.GetAsync("https://localhost:7001/api/ProductDetails/GetById/" + id);
                if (fb.IsSuccessStatusCode)
                {
                    var json = await fb.Content.ReadAsStringAsync();
                    var model = JsonConvert.DeserializeObject<GetByIdProductDetailDto>(json) ?? new GetByIdProductDetailDto();
                    return View(model);
                }
            }
            return View(new GetByIdProductDetailDto());
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProductDetail(string id)
        { 
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = GetCatalogBaseUrl();
            var resp = await client.DeleteAsync(baseUrl + "/api/ProductDetails/" + id);
            if (!resp.IsSuccessStatusCode)
            {
                await client.DeleteAsync("https://localhost:7001/api/ProductDetails/" + id);
            }
            return RedirectToAction("Index", "ProductDetail", new { area = "Admin" });
        }
    }
}
