using Dapper;
using MultiShop.Discount.Context;
using MultiShop.Discount.Dtos.DiscountCouponDtos;

namespace MultiShop.Discount.Services.DiscountServices;

public class DiscountService : IDiscountService
{
    private readonly MultiShopDiscountDapperContext _context;

    public DiscountService(MultiShopDiscountDapperContext context)
    {
        _context = context;
    }

    public async Task CreateDiscountCouponAsync(CreateDiscountCouponDto createDiscountCouponDto)
    {
        string query = "insert into Coupons(Code, Rate, IsActive, ValidDate) values (@code, @rate, @isActive, @validDate)";
        var parameters = new DynamicParameters();
        parameters.Add("@code", createDiscountCouponDto.Code);
        parameters.Add("@rate", createDiscountCouponDto.Rate);
        parameters.Add("@isActive", createDiscountCouponDto.IsActive);
        parameters.Add("@validDate", createDiscountCouponDto.ValidDate);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task DeleteDiscountCouponAsync(int id)
    {
        string query = "Delete From Coupons where CouponId=@couponId";
        var parameters = new DynamicParameters();
        parameters.Add("@couponId", id);
        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<List<ResultDiscountCouponDto>> GetAllDiscountCouponAsync()
    {
        string query = "Select * From Coupons";
        using (var connection = _context.CreateConnection())
        {
            var values = await connection.QueryAsync<ResultDiscountCouponDto>(query);
            return values.ToList();
        }
    }

    public async Task<GetByIdDiscountCouponDto> GetByIdDiscountCouponAsync(int id)
    {
        string query = "Select * From Coupons Where CouponId=@couponId";
        var parameters = new DynamicParameters();
        parameters.Add("@couponId", id);
        using (var connection = _context.CreateConnection())
        {
            return await connection.QueryFirstOrDefaultAsync<GetByIdDiscountCouponDto>(query, parameters);
        }
    }

    public async Task<GetByIdDiscountCouponDto> GetByCodeDiscountCouponAsync(string code)
    {
        string query = "Select * From Coupons Where Code=@code";
        var parameters = new DynamicParameters();
        parameters.Add("@code", code);
        using (var connection = _context.CreateConnection())
        {
            return await connection.QueryFirstOrDefaultAsync<GetByIdDiscountCouponDto>(query, parameters);
        }
    }

    public async Task UpdateDiscountCouponAsync(UpdateDiscountCouponDto updateDiscountCoupon)
    {
        string query = "Update Coupons Set Code=@code, Rate=@rate, IsActive=@isActive, ValidDate=@validDate where CouponID=@couponID";

        var parameters = new DynamicParameters();
        parameters.Add("@code", updateDiscountCoupon.Code);
        parameters.Add("@rate", updateDiscountCoupon.Rate);
        parameters.Add("@isActive", updateDiscountCoupon.IsActive);
        parameters.Add("@validDate", updateDiscountCoupon.ValidDate);
        parameters.Add("@couponId", updateDiscountCoupon.CouponId);

        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    public async Task<int> GetTotalDiscountCouponCountAsync()
    {
        const string query = "Select COUNT(1) From Coupons";
        using (var connection = _context.CreateConnection())
        {
            var count = await connection.ExecuteScalarAsync<int>(query);
            return count;
        }
    }

    public async Task<int> GetActiveDiscountCouponCountAsync()
    {
        const string query = "Select COUNT(1) From Coupons Where IsActive = 1";
        using (var connection = _context.CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }

    public async Task<int> GetInactiveDiscountCouponCountAsync()
    {
        const string query = "Select COUNT(1) From Coupons Where IsActive = 0";
        using (var connection = _context.CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }

    public async Task<int> GetExpiredDiscountCouponCountAsync()
    {
        const string query = "Select COUNT(1) From Coupons Where ValidDate < GETUTCDATE()";
        using (var connection = _context.CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }

    public async Task<int> GetNonExpiredDiscountCouponCountAsync()
    {
        const string query = "Select COUNT(1) From Coupons Where ValidDate >= GETUTCDATE()";
        using (var connection = _context.CreateConnection())
        {
            return await connection.ExecuteScalarAsync<int>(query);
        }
    }
}