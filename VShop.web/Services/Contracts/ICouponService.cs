using VShop.web.Models;

namespace VShop.web.Services.Contracts;

public interface ICouponService
{
    Task<CouponViewModel> GetDiscountCoupon(string couponCode, string token);
}
