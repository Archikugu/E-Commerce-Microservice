using Microsoft.AspNetCore.Mvc;

namespace MultiShop.WebUI.Controllers;

public class ProductListController : Controller
{
    public IActionResult Index(string id, string categoryId)
    {
        // Eğer categoryId varsa onu kullan, yoksa id'yi kullan
        var categoryIdToUse = !string.IsNullOrEmpty(categoryId) ? categoryId : id;
        ViewBag.id = categoryIdToUse;
        return View();
    }
    
    public IActionResult ProductDetail(string id)
    {
        ViewBag.id = id;
        return View();
    }
}
