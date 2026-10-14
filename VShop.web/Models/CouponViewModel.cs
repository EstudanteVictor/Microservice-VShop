namespace VShop.web.Models;

public class CouponViewModel
{
    public int Id { get; set; }
    public string? CouponCode { get; set; }
    public decimal Discount { get; set; } = 0.00m;
}
