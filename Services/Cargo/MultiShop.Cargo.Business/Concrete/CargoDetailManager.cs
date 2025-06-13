using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.Business.Concrete;

public class CargoDetailManager : ICargoDetailService
{
    private readonly ICargoDetailDal _cargoDetailDal;

    public CargoDetailManager(ICargoDetailDal cargoDetailDal)
    {
        _cargoDetailDal = cargoDetailDal;
    }

    public void TAdd(CargoDetail entity)
    {
        _cargoDetailDal.Add(entity);
    }

    public void TDelete(int id)
    {
        _cargoDetailDal.Delete(id);
    }

    public List<CargoDetail> TGetAll()
    {
        return _cargoDetailDal.GetAll();
    }

    public List<CargoDetail> TGetByFilter(Func<CargoDetail, bool> filter)
    {
        return _cargoDetailDal.GetByFilter(filter);
    }

    public CargoDetail TGetById(int id)
    {
        return _cargoDetailDal.GetById(id);
    }

    public void TUpdate(CargoDetail entity)
    {
        _cargoDetailDal.Update(entity);
    }
}
