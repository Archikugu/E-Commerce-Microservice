namespace MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;

public class GetByIdProductDto
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public int StockQuantity { get; set; }
    public int MinStock { get; set; }
    public int MaxStock { get; set; }
}
