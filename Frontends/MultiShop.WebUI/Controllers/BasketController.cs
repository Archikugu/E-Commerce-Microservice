using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Dtos.BasketDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;

namespace MultiShop.WebUI.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IProductService _productService;

        public BasketController(IBasketService basketService, IProductService productService)
        {
            _basketService = basketService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var basket = await _basketService.GetBasket();
            return View(basket);
        }

        [HttpGet("Basket/AddBasketItem/{id}")]
        public async Task<IActionResult> AddBasketItem(string id)
        {
            try
            {
                var product = await _productService.GetByIdProductAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Ürün bulunamadı!";
                    return RedirectToAction("Index");
                }

                var basketItem = new BasketItemDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    Quantity = 1
                };

                await _basketService.AddBasketItem(basketItem);
                TempData["SuccessMessage"] = "Ürün sepete eklendi!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Sepete ekleme hatası: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveBasketItem(string productId)
        {
            try
            {
                var result = await _basketService.RemoveBasketItem(productId);
                if (result)
                {
                    TempData["SuccessMessage"] = "Ürün sepetten çıkarıldı!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Ürün bulunamadı!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Sepetten çıkarma hatası: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveBasket(BasketTotalDto basketTotalDto)
        {
            try
            {
                await _basketService.SaveBasket(basketTotalDto);
                TempData["SuccessMessage"] = "Sepet kaydedildi!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Sepet kaydetme hatası: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ClearBasket()
        {
            try
            {
                var basket = await _basketService.GetBasket();
                if (basket != null)
                {
                    basket.BasketItems.Clear();
                    await _basketService.SaveBasket(basket);
                    TempData["SuccessMessage"] = "Sepet temizlendi!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Sepet temizleme hatası: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
