using Microsoft.EntityFrameworkCore;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Contexts;

public class MultiShopCargoContext : DbContext
{
    public MultiShopCargoContext(DbContextOptions<MultiShopCargoContext> options) : base(options) { }

    public DbSet<CargoCompany> CargoCompanies { get; set; }
    public DbSet<CargoDetail> CargoDetails { get; set; }
    public DbSet<CargoCustomer> CargoCustomers { get; set; }
    public DbSet<CargoOperation> CargoOperations { get; set; }
}
