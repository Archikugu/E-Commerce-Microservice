using Microsoft.EntityFrameworkCore;
using MultiShop.Comment.Entities;
namespace MultiShop.Comment.Context;

public class MultiShopCommentContext : DbContext
{
    public MultiShopCommentContext(DbContextOptions<MultiShopCommentContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1442;Database=MultiShopCommentDb;User Id=sa;Password=123456789aA*;TrustServerCertificate=true;");
        }
    }
    
    public DbSet<UserComment> UserComments { get; set; }
}
