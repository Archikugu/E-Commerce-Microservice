using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;
using System.Linq;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    //todo images upload işlemi tamamlanacak 
    public class ProductImageController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceAPISettings _apiSettings;
        private readonly IClientCrendentialTokenService _clientTokenService;

        public ProductImageController(IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiOptions.Value;
            _clientTokenService = clientTokenService;
        }

        private async Task<List<SelectListItem>> FetchProductSelectListAsync()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
            var products = new List<SelectListItem>();
            var resp = await client.GetAsync(baseUrl + "/api/Products");
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                var vals = JsonConvert.DeserializeObject<List<ResultProductDto>>(json) ?? new List<ResultProductDto>();
                products = vals.Select(x => new SelectListItem { Text = x.ProductName, Value = x.ProductId }).ToList();
            }
            else
            {
                var fb = await client.GetAsync("https://localhost:7001/api/Products");
                if (fb.IsSuccessStatusCode)
                {
                    var json = await fb.Content.ReadAsStringAsync();
                    var vals = JsonConvert.DeserializeObject<List<ResultProductDto>>(json) ?? new List<ResultProductDto>();
                    products = vals.Select(x => new SelectListItem { Text = x.ProductName, Value = x.ProductId }).ToList();
                }
            }
            return products;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.v1 = "Home";
            ViewBag.v2 = "Products";
            ViewBag.v3 = "Product Image List";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');

            var values = new List<ResultProductImageDto>();
            var responseMessage = await client.GetAsync(baseUrl + "/api/ProductImages");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                values = JsonConvert.DeserializeObject<List<ResultProductImageDto>>(jsonData) ?? new List<ResultProductImageDto>();
            }
            else
            {
                // Fallback: direct downstream (dev)
                var fallback = await client.GetAsync("https://localhost:7001/api/ProductImages");
                if (fallback.IsSuccessStatusCode)
                {
                    var jsonData = await fallback.Content.ReadAsStringAsync();
                    values = JsonConvert.DeserializeObject<List<ResultProductImageDto>>(jsonData) ?? new List<ResultProductImageDto>();
                }
            }

            // Products'ları getir (başarısız olsa da ViewBag'e boş liste koy)
            var responseMessage2 = await client.GetAsync(baseUrl + "/api/Products");
            if (responseMessage2.IsSuccessStatusCode)
            {
                var jsonData2 = await responseMessage2.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData2) ?? new List<ResultProductDto>();
                ViewBag.Products = products;
            }
            else
            {
                // Fallback
                var fallback2 = await client.GetAsync("https://localhost:7001/api/Products");
                if (fallback2.IsSuccessStatusCode)
                {
                    var jsonData2 = await fallback2.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData2) ?? new List<ResultProductDto>();
                    ViewBag.Products = products;
                }
                else
                {
                    ViewBag.Products = new List<ResultProductDto>();
                }
            }

            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> CreateProductImage()
        {
            // Product listesini getir
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
            var responseMessage = await client.GetAsync(baseUrl + "/api/Products");
            var products = new List<SelectListItem>();
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData) ?? new List<ResultProductDto>();
                products = values.Select(x => new SelectListItem { Text = x.ProductName, Value = x.ProductId.ToString() }).ToList();
            }
            else
            {
                var fallback = await client.GetAsync("https://localhost:7001/api/Products");
                if (fallback.IsSuccessStatusCode)
                {
                    var jsonData = await fallback.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData) ?? new List<ResultProductDto>();
                    products = values.Select(x => new SelectListItem { Text = x.ProductName, Value = x.ProductId.ToString() }).ToList();
                }
            }
            ViewBag.Products = products;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductImage(CreateProductImageDto createProductImageDto)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
            var jsonData = JsonConvert.SerializeObject(createProductImageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync(baseUrl + "/api/ProductImages", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductImage", new { area = "Admin" });
            }

            // Fallback: direct Catalog service (dev)
            var direct = await client.PostAsync("https://localhost:7001/api/ProductImages", stringContent);
            if (direct.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductImage", new { area = "Admin" });
            }

            var errText = await responseMessage.Content.ReadAsStringAsync();
            var errText2 = await direct.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errText2) ? (string.IsNullOrWhiteSpace(errText) ? "Product images could not be created." : errText) : errText2);
            ViewBag.Products = await FetchProductSelectListAsync();
            return View(createProductImageDto);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateProductImage(string id)
        {
            // Product listesini getir (fallback'lı yardımcı metot ile)
            var products = await FetchProductSelectListAsync();
            ViewBag.Products = products ?? new List<SelectListItem>();

            // ProductImage'ı getir
            var client1 = _httpClientFactory.CreateClient();
            client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
            var responseMessage1 = await client1.GetAsync(baseUrl + "/api/ProductImages/" + id);
            if (responseMessage1.IsSuccessStatusCode)
            {
                var jsonData1 = await responseMessage1.Content.ReadAsStringAsync();
                var values1 = JsonConvert.DeserializeObject<UpdateProductImageDto>(jsonData1) ?? new UpdateProductImageDto();
                return View(values1);
            }
            else
            {
                var fallback1 = await client1.GetAsync("https://localhost:7001/api/ProductImages/" + id);
                if (fallback1.IsSuccessStatusCode)
                {
                    var jsonData1 = await fallback1.Content.ReadAsStringAsync();
                    var values1 = JsonConvert.DeserializeObject<UpdateProductImageDto>(jsonData1) ?? new UpdateProductImageDto();
                    return View(values1);
                }
                return View(new UpdateProductImageDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductImage(UpdateProductImageDto updateProductImageDto)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
            var jsonData = JsonConvert.SerializeObject(updateProductImageDto);
            StringContent stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responseMessage = await client.PutAsync(baseUrl + "/api/ProductImages", stringContent);
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductImage", new { area = "Admin" });
            }

            // Fallback: direct Catalog service (dev)
            var direct = await client.PutAsync("https://localhost:7001/api/ProductImages", stringContent);
            if (direct.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "ProductImage", new { area = "Admin" });
            }

            var errText = await responseMessage.Content.ReadAsStringAsync();
            var errText2 = await direct.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errText2) ? (string.IsNullOrWhiteSpace(errText) ? "Product images could not be updated." : errText) : errText2);
            ViewBag.Products = await FetchProductSelectListAsync();
            return View(updateProductImageDto);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteProductImage(string id)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
                var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
                var responseMessage = await client.DeleteAsync(baseUrl + "/api/ProductImages/" + id);
                if (!responseMessage.IsSuccessStatusCode)
                {
                    // Fallback: direct downstream (dev)
                    await client.DeleteAsync("https://localhost:7001/api/ProductImages/" + id);
                }
            }
            catch
            {
                // swallow and continue to list to keep UX smooth
            }
            return RedirectToAction("Index", "ProductImage", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> ProductImageGallery(string productId)
        {
            ViewBag.v1 = "Home";
            ViewBag.v2 = "Products";
            ViewBag.v3 = "Product Image Gallery";
            ViewBag.ProductId = productId;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _clientTokenService.GetToken());
            var baseUrl = _apiSettings.OcelotUrl.TrimEnd('/') + "/" + _apiSettings.Catalog.Path.TrimStart('/').TrimEnd('/');
            var responseMessage = await client.GetAsync(baseUrl + $"/api/ProductImages/ProductImagesByProductId/{productId}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultProductImageDto>>(jsonData) ?? new List<ResultProductImageDto>();
                return View(values);
            }
            else
            {
                var fallback = await client.GetAsync($"https://localhost:7001/api/ProductImages/ProductImagesByProductId/{productId}");
                if (fallback.IsSuccessStatusCode)
                {
                    var jsonData = await fallback.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultProductImageDto>>(jsonData) ?? new List<ResultProductImageDto>();
                    return View(values);
                }
                return View(new List<ResultProductImageDto>());
            }

        }

        [HttpPost]
        [Area("Admin")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "File is required" });
            }

            try
            {
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

                // Fallback: direct Images service (dev) with relaxed certificate validation
                using var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
                using var directClient = new HttpClient(handler);
                using var content3 = new MultipartFormDataContent();
                using var stream3 = file.OpenReadStream();
                var sc3 = new StreamContent(stream3);
                sc3.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                content3.Add(sc3, "file", file.FileName);
                content3.Add(new StringContent(file.FileName), "fileName");
                var directUrl = "https://localhost:7008/api/GoogleDriveImageUploads/upload";
                var resp3 = await directClient.PostAsync(directUrl, content3);
                var body3 = await resp3.Content.ReadAsStringAsync();
                if (resp3.IsSuccessStatusCode)
                {
                    return Content(body3, "application/json");
                }

                return StatusCode((int)resp3.StatusCode, new { error = string.IsNullOrWhiteSpace(body3) ? "Upload failed (Ocelot + direct)." : body3 });
            }
            catch (Exception ex)
            {
                // If Ocelot request threw (e.g., SSL or network), try direct Images with relaxed cert
                try
                {
                    using var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
                    using var directClient = new HttpClient(handler);
                    using var content = new MultipartFormDataContent();
                    using var stream = file.OpenReadStream();
                    var sc = new StreamContent(stream);
                    sc.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                    content.Add(sc, "file", file.FileName);
                    content.Add(new StringContent(file.FileName), "fileName");
                    var directUrl = "https://localhost:7008/api/GoogleDriveImageUploads/upload";
                    var resp = await directClient.PostAsync(directUrl, content);
                    var body = await resp.Content.ReadAsStringAsync();
                    if (resp.IsSuccessStatusCode)
                    {
                        return Content(body, "application/json");
                    }
                    return StatusCode((int)resp.StatusCode, new { error = string.IsNullOrWhiteSpace(body) ? ex.Message : body });
                }
                catch (Exception ex2)
                {
                    return StatusCode(500, new { error = ex2.Message });
                }
            }
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

            try
            {
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

                // Fallback: direct Images service (dev) with relaxed certificate validation
                using var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
                using var directClient = new HttpClient(handler);
                var url3 = $"https://localhost:7008/api/GoogleDriveImageUploads/signed-url/{Uri.EscapeDataString(fileName)}?minutes={minutes}";
                var resp3 = await directClient.GetAsync(url3);
                var body3 = await resp3.Content.ReadAsStringAsync();
                if (resp3.IsSuccessStatusCode)
                {
                    return Content(body3, "application/json");
                }

                return StatusCode((int)resp3.StatusCode, new { error = string.IsNullOrWhiteSpace(body3) ? "Signed URL request failed (Ocelot + direct)." : body3 });
            }
            catch (Exception ex)
            {
                try
                {
                    using var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
                    using var directClient = new HttpClient(handler);
                    var url3 = $"https://localhost:7008/api/GoogleDriveImageUploads/signed-url/{Uri.EscapeDataString(fileName)}?minutes={minutes}";
                    var resp3 = await directClient.GetAsync(url3);
                    var body3 = await resp3.Content.ReadAsStringAsync();
                    if (resp3.IsSuccessStatusCode)
                    {
                        return Content(body3, "application/json");
                    }
                    return StatusCode((int)resp3.StatusCode, new { error = string.IsNullOrWhiteSpace(body3) ? ex.Message : body3 });
                }
                catch (Exception ex2)
                {
                    return StatusCode(500, new { error = ex2.Message });
                }
            }
        }
    }
}