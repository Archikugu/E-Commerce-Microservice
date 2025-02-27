namespace MultiShop.Discount.Dtos.DiscountCouponDtos;

public class GetByIdDiscountCouponDto
{
    public int CouponId { get; set; }
    public string Code { get; set; }
    public int Rate { get; set; }
    public bool IsActive { get; set; }
    public DateTime ValidDate { get; set; }
}
