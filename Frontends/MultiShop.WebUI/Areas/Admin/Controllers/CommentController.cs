using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using MultiShop.WebUI.Dtos.CommentDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.CommentServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IProductService _productService;

        public CommentController(ICommentService commentService, IProductService productService)
        {
            _commentService = commentService;
            _productService = productService;
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
    }
}
