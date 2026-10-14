using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VShop.DiscountApi.Context;
using VShop.DiscountApi.DTOs;

namespace VShop.DiscountApi.Repositories;

public class CouponRepository : ICouponRepository
{
    private IMapper _mapper;
    private readonly AppDbContext _context;

    public CouponRepository(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
     }

    public async Task<CouponDTO> GetCouponByCode(string couponCode)
    {
        var coupon = await _context.coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);
        return _mapper.Map<CouponDTO>(coupon);
    }
}
