using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VShop.web.Models;
using VShop.web.Services.Contracts;

namespace VShop.web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly ICouponService _couponService;

    public CartController(ICartService cartService, ICouponService couponService)
    {
        _cartService = cartService;
        _couponService = couponService;
    }

    [Authorize]
    public async Task< IActionResult> Index()
    {
        CartViewModel cartViewModel = await GetCartByUser();

        if(cartViewModel is null)
        {
            ModelState.AddModelError("CartNotFound", "Cart yet doesen't exist...");
            return View("/Views/Cart/CartNotFound.cshtml");
        }

        return View(cartViewModel);
    }

    public async Task<IActionResult> RemoveItem(int id)
    {
        var result = await _cartService.DeleteItemCartAsync(id, await GetAccessToken());
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(id);
    }

    [HttpPost]
    public async Task<IActionResult> ApplyCoupon(CartViewModel cartView)
    {
        if (ModelState.IsValid)
        {
            var result = await _cartService.ApplyCouponAsync(cartView, await GetAccessToken());
            if (result)
                return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCoupon()
    {
        if (ModelState.IsValid)
        {
            var result = await _cartService.DeleteCouponAsync(GetUserId(), await GetAccessToken());
            if (result)
                return RedirectToAction(nameof(Index));
        }
        return View();
    }

    private async Task<CartViewModel> GetCartByUser()
    {
        var cart = await _cartService.GetCartByUserIdAsync(GetUserId(), await GetAccessToken());
        if(cart?.CartHeader is not null) 
        {
            if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
            {
                var coupon = await _couponService.GetDiscountCoupon(cart.CartHeader.CouponCode,await GetAccessToken());
                if (coupon is not null)
                {
                    if (coupon.CouponCode is not null)
                    {
                        cart.CartHeader.Discount = coupon.Discount;
                    }
                }
            }

            foreach (var item in cart.CartItems)
            {
                cart.CartHeader.TotalAmount += (item.Product.Price * item.Quantity);
            }

            cart.CartHeader.TotalAmount = cart.CartHeader.TotalAmount - (cart.CartHeader.TotalAmount * cart.CartHeader.Discount)/100; 
        }
        return cart;
    }

    private async Task<string> GetAccessToken()
    {
        return await HttpContext.GetTokenAsync("access_token");
    }

    private string GetUserId()
    {
        return User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
    }
}

