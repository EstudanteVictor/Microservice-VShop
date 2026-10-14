using VShop.web.Models;

namespace VShop.web.Services.Contracts;

public interface ICartService
{
    Task<CartViewModel> GetCartByUserIdAsync(string userId, string token);
    Task<CartViewModel> AddItemToCartAsync(CartViewModel cartVM, string token);
    Task<CartViewModel> UpdateCartAsync(CartViewModel cartVM, string token);
    Task<bool> DeleteItemCartAsync(int cartId, string token);

    Task<bool> CleanCartAsync(string userId, string token);
    Task<bool> ApplyCouponAsync(CartViewModel cartVM, string token);
    Task<bool> DeleteCouponAsync(string userId, string token);

    Task<CartViewModel> CheckoutAsync(CartHeaderViewModel cartHeader, string token);
}
