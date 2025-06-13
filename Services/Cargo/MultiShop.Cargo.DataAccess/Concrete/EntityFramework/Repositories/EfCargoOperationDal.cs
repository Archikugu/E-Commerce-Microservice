using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Contexts;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Repositories;

public class EfCargoOperationDal : GenericRepository<CargoOperation>, ICargoOperationDal
{
    public EfCargoOperationDal(MultiShopCargoContext context) : base(context)
    {
    }
}
