using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Dtos.BasketDtos;
using System.Text.Json;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
public class WishlistController : Controller
{
    private const string WishlistSessionKey = "user_wishlist_ids";
    private readonly IProductService _productService;
    private readonly IBasketService _basketService;

    public WishlistController(IProductService productService, IBasketService basketService)
    {
        _productService = productService;
        _basketService = basketService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var ids = GetIds();
        var list = new List<MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos.GetByIdProductDto>();
        foreach (var pid in ids)
        {
            try
            {
                var p = await _productService.GetByIdProductAsync(pid);
                if (p != null) list.Add(p);
            }
            catch { /* ignore */ }
        }
        return View(list);
    }

    [HttpPost]
    public IActionResult Add(string id, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            var ids = GetIds();
            if (!ids.Contains(id))
            {
                ids.Add(id);
                SaveIds(ids);
            }
        }
        return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? Url.Action("Index")! : returnUrl);
    }

    [HttpPost]
    public IActionResult Remove(string id, string? returnUrl = null)
    {
        var ids = GetIds();
        if (!string.IsNullOrWhiteSpace(id) && ids.Remove(id))
        {
            SaveIds(ids);
        }
        return Redirect(string.IsNullOrWhiteSpace(returnUrl) ? Url.Action("Index")! : returnUrl);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(string id, string? returnUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            try
            {
                var product = await _productService.GetByIdProductAsync(id);
                if (product != null)
                {
                    var basketItem = new BasketItemDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = 1
                    };
                    await _basketService.AddBasketItem(basketItem);
                    // remove from wishlist
                    var ids = GetIds();
                    if (ids.Remove(id)) SaveIds(ids);
                }
            }
            catch { /* ignore */ }
        }
        if (!string.IsNullOrWhiteSpace(returnUrl)) return Redirect(returnUrl);
        return RedirectToAction("Index", "Basket", new { area = "" });
    }

    private List<string> GetIds()
    {
        var json = HttpContext.Session.GetString(WishlistSessionKey);
        if (string.IsNullOrWhiteSpace(json)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>(); }
        catch { return new List<string>(); }
    }

    private void SaveIds(List<string> ids)
    {
        var json = JsonSerializer.Serialize(ids.Distinct().ToList());
        HttpContext.Session.SetString(WishlistSessionKey, json);
    }
}


