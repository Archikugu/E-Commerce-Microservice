namespace MultiShop.Order.Domain.Entities;

public class Ordering
{
    public int OrderingId { get; set; }
    public string UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "New"; // New, Processing, Shipped, Delivered, Cancelled
    public List<OrderDetail> OrderDetails { get; set; }
}