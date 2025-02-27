using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MultiShop.Discount.Entities;
using System.Data;

namespace MultiShop.Discount.Context;

public class MultiShopDiscountDapperContext : DbContext
{
    private readonly string _connectionString;

    public MultiShopDiscountDapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    public DbSet<Coupon> Coupons { get; set; }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}
