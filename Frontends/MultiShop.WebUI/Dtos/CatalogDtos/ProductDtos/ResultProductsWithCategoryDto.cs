using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;

namespace MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
public class ResultProductsWithCategoryDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public ResultCategoryDto Category { get; set; }
}
