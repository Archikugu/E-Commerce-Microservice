namespace MultiShop.Catalog.Dtos.ProductImageDtos;

public class ProductImageSliderDto
{
    public string ProductId { get; set; }
    public List<string> ImageUrls { get; set; }
    public int TotalImageCount { get; set; }
    public bool HasImages => ImageUrls?.Any() == true;
}
