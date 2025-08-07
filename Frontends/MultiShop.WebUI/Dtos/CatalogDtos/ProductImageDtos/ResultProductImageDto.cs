namespace MultiShop.WebUI.Dtos.CatalogDtos.ProductImageDtos;

public class ResultProductImageDto
{
    public string ProductImageId { get; set; }
    public List<string> Images { get; set; } = new List<string>();
    public string ProductId { get; set; }
}