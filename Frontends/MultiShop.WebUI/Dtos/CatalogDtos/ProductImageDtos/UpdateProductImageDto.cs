namespace MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;

public class UpdateProductImageDto
{
    public string ProductImageId { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public string ProductId { get; set; }
}