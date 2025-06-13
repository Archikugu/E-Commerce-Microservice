using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.Business.Concrete;

public class CargoCompanyManager : ICargoCompanyService
{
    private readonly ICargoCompanyDal _cargoCompanyDal;

    public CargoCompanyManager(ICargoCompanyDal cargoCompanyDal)
    {
        _cargoCompanyDal = cargoCompanyDal;
    }

    public void TAdd(CargoCompany entity)
    {
        _cargoCompanyDal.Add(entity);
    }

    public void TDelete(int id)
    {
        _cargoCompanyDal.Delete(id);
    }

    public List<CargoCompany> TGetAll()
    {
        return _cargoCompanyDal.GetAll();
    }

    public List<CargoCompany> TGetByFilter(Func<CargoCompany, bool> filter)
    {
        return _cargoCompanyDal.GetByFilter(filter);
    }

    public CargoCompany TGetById(int id)
    {
        return _cargoCompanyDal.GetById(id);
    }

    public void TUpdate(CargoCompany entity)
    {
        _cargoCompanyDal.Update(entity);
    }
}
