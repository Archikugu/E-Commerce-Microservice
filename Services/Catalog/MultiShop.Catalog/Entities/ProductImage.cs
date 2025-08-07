using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MultiShop.Catalog.Entities;

public class ProductImage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ProductImageId { get; set; }
    
    public List<string> Images { get; set; } = new List<string>();
    
    // Backward compatibility için eski alanları koruyalım
    [BsonIgnoreIfDefault]
    public string Image1 { get; set; }
    [BsonIgnoreIfDefault]
    public string Image2 { get; set; }
    [BsonIgnoreIfDefault]
    public string Image3 { get; set; }
    
    public string ProductId { get; set; }
    [BsonIgnore]
    public Product Product { get; set; }

    // Eski alanları yeni formata dönüştürme metodu
    [BsonIgnore]
    public List<string> AllImages
    {
        get
        {
            var allImages = new List<string>();
            
            // Önce yeni format (Images array)
            if (Images != null && Images.Any())
            {
                allImages.AddRange(Images);
            }
            // Sonra eski format
            else
            {
                if (!string.IsNullOrEmpty(Image1)) allImages.Add(Image1);
                if (!string.IsNullOrEmpty(Image2)) allImages.Add(Image2);
                if (!string.IsNullOrEmpty(Image3)) allImages.Add(Image3);
            }
            
            return allImages;
        }
    }
}