using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VShop.CartApi.Context;
using VShop.CartApi.DTOs;
using VShop.CartApi.Models;

namespace VShop.CartApi.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private IMapper _mapper;

        public CartRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartByUserIdAsync(string userId)
        {
            Cart cart = new Cart()
            {
                CartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId)
            };

            cart.CartItems = _context.CartItems.Where(c => c.CartHeaderId == cart.CartHeader.Id)
                .Include(p => p.Product);

            return _mapper.Map<CartDTO>(cart);
        }

        public async Task<bool> DeleteItemCartAsync(int cartItemId)
        {
            try
            {
                CartItem cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);

                int total = _context.CartItems.Where(c => c.CartHeaderId == cartItem.CartHeaderId).Count();

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                if (total == 1)
                {
                    var cartHeaderRemove = await _context.CartHeaders.FirstOrDefaultAsync(
                        c => c.Id == cartItem.CartHeaderId);

                    _context.CartHeaders.Remove(cartHeaderRemove);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<bool> CleanCartAsync(string userId)
        {
            try
            {
                var cartHeader = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

                if (cartHeader is not null)
                {
                    _context.CartItems.RemoveRange(_context.CartItems.Where(c => c.CartHeaderId == cartHeader.Id));

                    _context.CartHeaders.Remove(cartHeader);

                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<CartDTO> UpdateCartAsync(CartDTO cartDto)
        {
            Cart cart1 = _mapper.Map<Cart>(cartDto);

            await SaveProductsInDataBase(cartDto, cart1);

            var cartHeader = await _context.CartHeaders.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == cart1.CartHeader.UserId);

            if (cartHeader is null)
                //criar header e os itens
                await CreateCartHeaderAndItems(cart1);

            else
                await UpdateQuantityAndItems(cartDto, cart1, cartHeader);

            return _mapper.Map<CartDTO>(cart1);
        }

        private async Task UpdateQuantityAndItems(CartDTO cartDto, Cart cart, CartHeader? cartHeader)
        {
            //se CartHeader não é null
            //verifica se CartItems possui o mesmo produto
            var cartDetail = await _context.CartItems.AsNoTracking().FirstOrDefaultAsync(
                p => p.ProductId == cartDto.CartItems.FirstOrDefault()
                .ProductId && p.CartHeaderId == cartHeader.Id);

            if (cartDetail is null)
            {
                cart.CartItems.FirstOrDefault().CartHeaderId = cartHeader.Id;
                cart.CartItems.FirstOrDefault().Product = null;
                await _context.CartItems.AddAsync(cart.CartItems.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
            else
            {
                cart.CartItems.FirstOrDefault().Product = null;
                cart.CartItems.FirstOrDefault().Quantity += cartDetail.Quantity;
                cart.CartItems.FirstOrDefault().Id = cartDetail.Id;
                cart.CartItems.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
                _context.CartItems.Update(cart.CartItems.FirstOrDefault());
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateCartHeaderAndItems(Cart cart)
        {
            await _context.CartHeaders.AddAsync(cart.CartHeader);
            await _context.SaveChangesAsync();

            cart.CartItems.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartItems.FirstOrDefault().Product = null;

            await _context.CartItems.AddAsync(cart.CartItems.FirstOrDefault());

            await _context.SaveChangesAsync();
        }

        private async Task SaveProductsInDataBase(CartDTO cartDto, Cart cart1)
        {
            //verifica se produto já foi salvo, se não, salva
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == cartDto.CartItems.FirstOrDefault().ProductId);

            if (product is null)
            {
                await _context.Products.AddAsync(cart1.CartItems.FirstOrDefault().Product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ApplyCouponAsync(string userId, string couponCode)
        {
            var cartHeaderApplyCoupon = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cartHeaderApplyCoupon is not null)
            {
                cartHeaderApplyCoupon.CouponCode = couponCode;

                _context.CartHeaders.Update(cartHeaderApplyCoupon);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteCouponAsync(string userId)
        {
            var cartHeaderApplyCoupon = await _context.CartHeaders.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cartHeaderApplyCoupon is not null)
            {
                cartHeaderApplyCoupon.CouponCode = string.Empty;

                _context.CartHeaders.Update(cartHeaderApplyCoupon);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

    }
}
