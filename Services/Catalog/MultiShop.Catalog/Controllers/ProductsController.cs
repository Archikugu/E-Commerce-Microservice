using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Catalog.Dtos.ProductDtos;
using MultiShop.Catalog.Services.ProductServices;

namespace MultiShop.Catalog.Controllers;

//[Authorize]
[AllowAnonymous] // For testing purposes, remove in production
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
    public async Task<IActionResult> ProductsList()
    {
        var values = await _ProductsService.GetAllProductAsync();
        return Ok(values);
    }
    [HttpGet("{id}")]
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

    [HttpGet("GetProductsWithCategory")]
    public async Task<IActionResult> GetProductsWithCategory()
    {
        var values = await _ProductsService.GetProductsWithCategoryAsync();
        return Ok(values);
    }
}
