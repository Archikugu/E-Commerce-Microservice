using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MultiShop.Catalog.Entities;

public class About
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string AboutId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string WorkingHours { get; set; }
} 