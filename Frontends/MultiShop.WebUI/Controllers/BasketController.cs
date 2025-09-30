using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Dtos.BasketDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using System.Linq;

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
                    TempData["ErrorMessage"] = "Product not found.";
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
                TempData["SuccessMessage"] = "Item added to cart.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Add to cart error: {ex.Message}";
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
                    TempData["SuccessMessage"] = "Item removed from cart.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Product not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Remove from cart error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SaveBasket(BasketTotalDto basketTotalDto)
        {
            try
            {
                await _basketService.SaveBasket(basketTotalDto);
                TempData["SuccessMessage"] = "Cart saved.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Cart save error: {ex.Message}";
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
                    TempData["SuccessMessage"] = "Cart cleared.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Cart clear error: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(string productId, int quantity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest(new { message = "Geçersiz ürün." });
                }
                if (quantity < 1) quantity = 1;

                var basket = await _basketService.GetBasket();
                if (basket == null)
                {
                    return NotFound(new { message = "Sepet bulunamadı." });
                }

                var item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);
                if (item == null)
                {
                    return NotFound(new { message = "Ürün sepetinizde bulunamadı." });
                }

                item.Quantity = quantity;
                await _basketService.SaveBasket(basket);

                var itemTotal = item.Price * item.Quantity;
                var subtotal = basket.TotalPrice;
                var rate = basket.DiscountRate ?? 0;
                var total = rate > 0 ? subtotal * (100 - rate) / 100 : subtotal;

                return Json(new
                {
                    ok = true,
                    quantity = item.Quantity,
                    itemTotal,
                    subtotal,
                    discountRate = rate,
                    total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
