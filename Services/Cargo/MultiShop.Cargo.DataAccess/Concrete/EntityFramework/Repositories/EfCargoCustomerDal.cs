using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Contexts;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Repositories;

public class EfCargoCustomerDal : GenericRepository<CargoCustomer>, ICargoCustomerDal
{
    private readonly MultiShopCargoContext _context;
    public EfCargoCustomerDal(MultiShopCargoContext context) : base(context)
    {
        _context = context;
    }


    public CargoCustomer GetCargoCustomerById(string userCustomerId)
    {
        var values = _context.CargoCustomers.Where(c => c.UserCustomerId == userCustomerId).FirstOrDefault();
        return values;
    }
}
