using Microsoft.EntityFrameworkCore;
using MultiShop.Order.Domain.Entities;

namespace MultiShop.Order.Persistence.Context
{
    public class MultiShopOrderContext : DbContext
    {
        public MultiShopOrderContext(DbContextOptions<MultiShopOrderContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Ordering> Orderings { get; set; }
    }
}