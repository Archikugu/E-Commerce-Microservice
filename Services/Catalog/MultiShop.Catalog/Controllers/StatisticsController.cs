using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Services.StatisticServices;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticService _statisticService;

    public StatisticsController(IStatisticService statisticService)
    {
        _statisticService = statisticService;
    }

    [HttpGet("counts")] // categories/products/brands counts
    public async Task<IActionResult> GetCounts()
    {
        var categoryCount = await _statisticService.GetCategoryCountAsync();
        var productCount = await _statisticService.GetProductCountAsync();
        var brandCount = await _statisticService.GetBrandCountAsync();
        return Ok(new { categoryCount, productCount, brandCount });
    }

    [HttpGet("category-count")]
    public async Task<IActionResult> GetCategoryCount()
    {
        var count = await _statisticService.GetCategoryCountAsync();
        return Ok(count);
    }

    [HttpGet("product-count")]
    public async Task<IActionResult> GetProductCount()
    {
        var count = await _statisticService.GetProductCountAsync();
        return Ok(count);
    }

    [HttpGet("brand-count")]
    public async Task<IActionResult> GetBrandCount()
    {
        var count = await _statisticService.GetBrandCountAsync();
        return Ok(count);
    }

    [HttpGet("average-product-price")] 
    public async Task<IActionResult> GetAverageProductPrice()
    {
        var average = await _statisticService.GetAverageProductPriceAsync();
        return Ok(average);
    }

    [HttpGet("cheapest-product")] 
    public async Task<IActionResult> GetCheapestProduct()
    {
        var product = await _statisticService.GetCheapestProductAsync();
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("most-expensive-product")] 
    public async Task<IActionResult> GetMostExpensiveProduct()
    {
        var product = await _statisticService.GetMostExpensiveProductAsync();
        if (product == null) return NotFound();
        return Ok(product);
    }
}


