using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Dtos.CommentDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.CommentServices;
using Microsoft.Extensions.Options;
using MultiShop.WebUI.Settings;
using System.Net.Http.Headers;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IProductService _productService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ServiceAPISettings _apiSettings;
        private readonly IClientCrendentialTokenService _clientTokenService;

        public CommentController(ICommentService commentService, IProductService productService, IHttpClientFactory httpClientFactory, IOptions<ServiceAPISettings> apiOptions, IClientCrendentialTokenService clientTokenService)
        {
            _commentService = commentService;
            _productService = productService;
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiOptions.Value;
            _clientTokenService = clientTokenService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.v1 = "Home";
            ViewBag.v2 = "Comments";
            ViewBag.v3 = "Comment List";

            var values = await _commentService.GetAllAsync();

            var products = await _productService.GetAllProductAsync();
            ViewBag.Products = products;
            ViewBag.ProductDict = products.ToDictionary(p => p.ProductId, p => p.ProductName);

            // Ürün -> Kategori adı sözlüğü
            var productsWithCategory = await _productService.GetProductsWithCategoryAsync();
            ViewBag.ProductCategoryDict = productsWithCategory.ToDictionary(p => p.ProductId, p => p.Category.CategoryName);

            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> CreateComment()
        {
            var products = await _productService.GetAllProductAsync();
            List<SelectListItem> productItems = products.Select(x => new SelectListItem
            {
                Text = x.ProductName,
                Value = x.ProductId
            }).ToList();
            ViewBag.Products = productItems;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto)
        {
            await _commentService.CreateAsync(createCommentDto);
            return RedirectToAction("Index", "Comment", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateComment(int id)
        {
            var products = await _productService.GetAllProductAsync();
            List<SelectListItem> productItems = products.Select(x => new SelectListItem
            {
                Text = x.ProductName,
                Value = x.ProductId
            }).ToList();
            ViewBag.Products = productItems;

            var value = await _commentService.GetByIdAsync(id.ToString());
            var model = new UpdateCommentDto
            {
                UserCommentId = value.UserCommentId,
                ProductId = value.ProductId,
                FirstName = value.FirstName,
                LastName=value.LastName,
                Email = value.Email,
                ImageUrl = value.ImageUrl,
                CommentDetail = value.CommentDetail,
                Rating = value.Rating,
                Status = value.Status,
                CreatedDate = value.CreatedDate
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateComment(UpdateCommentDto updateCommentDto)
        {
            await _commentService.UpdateAsync(updateCommentDto);
            return RedirectToAction("Index", "Comment", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(int id)
        {
            await _commentService.DeleteAsync(id.ToString());
            return RedirectToAction("Index", "Comment", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCommentStatus(int id, bool status)
        {
            var value = await _commentService.GetByIdAsync(id.ToString());
            var update = new UpdateCommentDto
            {
                UserCommentId = value.UserCommentId,
                ProductId = value.ProductId,
                FirstName = value.FirstName,
                LastName = value.LastName,
                Email = value.Email,
                ImageUrl = value.ImageUrl,
                CommentDetail = value.CommentDetail,
                Rating = value.Rating,
                Status = status,
                CreatedDate = value.CreatedDate
            };
            await _commentService.UpdateAsync(update);
            return RedirectToAction("Index", "Comment", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsByProduct(string productId)
        {
            ViewBag.v1 = "Home";
            ViewBag.v2 = "Comments";
            ViewBag.v3 = "Product Comments";

            var values = await _commentService.GetByProductIdAsync(productId);
            return View(values);
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
}
