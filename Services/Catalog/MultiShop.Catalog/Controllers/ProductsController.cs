using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Services.ProductServices;

namespace MultiShop.Catalog.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _ProductsService;

    public ProductsController(IProductService ProductsService)
    {
        _ProductsService = ProductsService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> ProductsList()
    {
        var values = await _ProductsService.GetAllProductAsync();
        return Ok(values);
    }
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsById(string id)
    {
        var values = await _ProductsService.GetByIdProductAsync(id);
        return Ok(values);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProducts(CreateProductDto createProductsDto)
    {
        await _ProductsService.CreateProductAsync(createProductsDto);
        return Ok("Product Successfully Added");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProducts(string id)
    {
        await _ProductsService.DeleteProductAsync(id);
        return Ok("Product Successfully Deleted");
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProducts(UpdateProductDto updateProductsDto)
    {
        await _ProductsService.UpdateProductAsync(updateProductsDto);
        return Ok("Product Successfully Updated");
    }

    public class DecrementStockRequest { public string ProductId { get; set; } public int Quantity { get; set; } }

    [HttpPost("DecrementStock")]
    [AllowAnonymous]
    public async Task<IActionResult> DecrementStock([FromBody] DecrementStockRequest req)
    {
        if (req == null || string.IsNullOrWhiteSpace(req.ProductId) || req.Quantity <= 0)
            return BadRequest();
        var ok = await _ProductsService.DecrementStockAsync(req.ProductId, req.Quantity);
        try
        {
            var root = Directory.GetCurrentDirectory();
            var logDir = Path.Combine(root, "wwwroot", "logs");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, "catalog-decrement.log");
            var line = $"{DateTime.UtcNow:O}\t{req.ProductId}\t-{req.Quantity}\t{(ok ? "OK" : "FAIL")}";
            System.IO.File.AppendAllLines(logFile, new[] { line });
        }
        catch { }
        if (!ok) return Conflict("Insufficient stock or product not found");
        return Ok(new { productId = req.ProductId, changed = ok });
    }

    [HttpGet("GetProductsWithCategory")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsWithCategory()
    {
        var values = await _ProductsService.GetProductsWithCategoryAsync();
        return Ok(values);
    }

    [HttpGet("GetProductsWithCategoryByCategoryId/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsWithCategoryByCategoryId(string categoryId)
    {
        var values = await _ProductsService.GetProductsWithCategoryByCategoryIdAsync(categoryId);
        return Ok(values);
    }
}
