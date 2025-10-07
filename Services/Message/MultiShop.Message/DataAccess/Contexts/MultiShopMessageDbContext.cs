using Microsoft.EntityFrameworkCore;

namespace MultiShop.Message.DataAccess.Contexts
{
    public class MultiShopMessageDbContext : DbContext
    {
        public MultiShopMessageDbContext(DbContextOptions<MultiShopMessageDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.Message> Messages { get; set; }
    }
}
