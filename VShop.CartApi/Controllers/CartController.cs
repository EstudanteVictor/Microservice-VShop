using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using VShop.CartApi.DTOs;
using VShop.CartApi.Repositories;

namespace VShop.CartApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    [HttpGet("getcart/{userId}")]
    public async Task<ActionResult<CartDTO>> GetByUserId(string userId)
    {
        var cartDto = await _cartRepository.GetCartByUserIdAsync(userId);

        if (cartDto is null)
            return NotFound();

        return Ok(cartDto);
    }

    [HttpPost("addcart")]
    public async Task<ActionResult<CartDTO>> AddCart([FromBody] CartDTO cartDTO)
    {
        var cartDto = await _cartRepository.UpdateCartAsync(cartDTO);

        if (cartDto is null)
            return NotFound();

        return Ok(cartDto);
    }

    [HttpPost("updatecart")]
    public async Task<ActionResult<CartDTO>> UpdateCart([FromBody] CartDTO cartDTO)
    {
        var cartDto = await _cartRepository.UpdateCartAsync(cartDTO);

        if (cartDto is null)
            return NotFound();

        return Ok(cartDto);
    }

    [HttpDelete("deletecart/{id}")]
    public async Task<ActionResult<bool>> DeleteCart(int id)
    {
        var status = await _cartRepository.DeleteItemCartAsync(id);
        if (!status) return BadRequest();
        return Ok(status);
    }

    [HttpPost("applycoupon")]
    public async Task<ActionResult<CartDTO>> ApplyCoupon(CartDTO cartDTO)
    {
        var result = await _cartRepository.ApplyCouponAsync(cartDTO.CartHeader.UserId, cartDTO.CartHeader.CouponCode);

        if (!result)
            return NotFound($"CartHeader not found for user id {cartDTO.CartHeader.UserId}");

        return Ok(result);
    }

    [HttpDelete("deletecoupon/{userId}")]
    public async Task<ActionResult<CartDTO>> DeleteCoupon(string userId)
    {
         var result = await _cartRepository.DeleteCouponAsync(userId);

        if (!result)
            return NotFound($"Discount coupon not found for user id {userId}");

        return Ok(result);
    }
}
