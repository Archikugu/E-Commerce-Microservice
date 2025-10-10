using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.Business.Concrete;

public class CargoCustomerManager : ICargoCustomerService
{
    private readonly ICargoCustomerDal _cargoCustomerDal;

    public CargoCustomerManager(ICargoCustomerDal cargoCustomerDal)
    {
        _cargoCustomerDal = cargoCustomerDal;
    }

    public void TAdd(CargoCustomer entity)
    {
        _cargoCustomerDal.Add(entity);
    }

    public void TDelete(int id)
    {
        _cargoCustomerDal.Delete(id);
    }

    public List<CargoCustomer> TGetAll()
    {
        return _cargoCustomerDal.GetAll();
    }

    public List<CargoCustomer> TGetByFilter(Func<CargoCustomer, bool> filter)
    {
        return _cargoCustomerDal.GetByFilter(filter);
    }

    public CargoCustomer TGetById(int id)
    {
        return _cargoCustomerDal.GetById(id);
    }

    public CargoCustomer TGetCargoCustomerById(string userCustomerId)
    {
        return _cargoCustomerDal.GetCargoCustomerById(userCustomerId);
    }

    public void TUpdate(CargoCustomer entity)
    {
        _cargoCustomerDal.Update(entity);
    }
}
