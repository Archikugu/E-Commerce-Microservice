using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Contexts;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Repositories;

public class EfCargoDetailDal : GenericRepository<CargoDetail>, ICargoDetailDal
{
    public EfCargoDetailDal(MultiShopCargoContext context) : base(context)
    {
    }
}
