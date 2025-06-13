using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.Dtos.Dtos.CargoCustomerDtos;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CargoCustomersController : ControllerBase
{
    private readonly ICargoCustomerService _cargoCustomerService;

    public CargoCustomersController(ICargoCustomerService cargoCustomerService)
    {
        _cargoCustomerService = cargoCustomerService;
    }

    [HttpGet]
    public IActionResult CargoCustomerList()
    {
        var cargoCustomers = _cargoCustomerService.TGetAll();
        return Ok(cargoCustomers);
    }

    [HttpGet("{id}")]
    public IActionResult GetCargoCustomerById(int id)
    {
        var values = _cargoCustomerService.TGetById(id);
        return Ok(values);
    }

    [HttpPost]
    public IActionResult CreateCargoCustomer(CreateCargoCustomerDto createCargoCustomerDto)
    {
        CargoCustomer createCargoCustomer = new CargoCustomer
        {
            Address = createCargoCustomerDto.Address,
            City = createCargoCustomerDto.City,
            District = createCargoCustomerDto.District,
            Email = createCargoCustomerDto.Email,
            FirstName = createCargoCustomerDto.FirstName,
            LastName = createCargoCustomerDto.LastName,
            PhoneNumber = createCargoCustomerDto.PhoneNumber,
        };

        _cargoCustomerService.TAdd(createCargoCustomer);
        return Ok("Cargo customer created successfully");
    }

    [HttpDelete]
    public IActionResult DeleteCargoCustomer(int id)
    {
        _cargoCustomerService.TDelete(id);
        return Ok("Cargo customer deleted successfully");
    }



    [HttpPut]
    public IActionResult UpdateCargoCustomer(UpdateCargoCustomerDto updateCargoCustomerDto)
    {
        CargoCustomer updateCargoCustomer = new CargoCustomer
        {
            CargoCustomerId = updateCargoCustomerDto.CargoCustomerId,
            Address = updateCargoCustomerDto.Address,
            City = updateCargoCustomerDto.City,
            District = updateCargoCustomerDto.District,
            Email = updateCargoCustomerDto.Email,
            FirstName = updateCargoCustomerDto.FirstName,
            LastName = updateCargoCustomerDto.LastName,
            PhoneNumber = updateCargoCustomerDto.PhoneNumber,
        };

        _cargoCustomerService.TUpdate(updateCargoCustomer);
        return Ok("Cargo customer update successfully");
    }

}
