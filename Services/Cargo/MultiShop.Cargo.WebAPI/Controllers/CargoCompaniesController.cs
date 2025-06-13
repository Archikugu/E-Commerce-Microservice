using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.Dtos.Dtos.CargoCompanyDtos;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CargoCompaniesController : ControllerBase
{
    private readonly ICargoCompanyService _cargoCompanyService;

    public CargoCompaniesController(ICargoCompanyService cargoCompanyService)
    {
        _cargoCompanyService = cargoCompanyService;
    }

    [HttpGet]
    public IActionResult CargoCompanyList()
    {
        var cargoCompanies = _cargoCompanyService.TGetAll();
        return Ok(cargoCompanies);
    }

    [HttpPost]
    public IActionResult CreateCargoCompany(CreateCargoCompanyDto createCargoCompanyDto)
    {
        CargoCompany createCargoCompany = new CargoCompany
        {
            CargoCompanyName = createCargoCompanyDto.CargoCompanyName,
        };

        _cargoCompanyService.TAdd(createCargoCompany);
        return Ok("Cargo company created successfully");
    }

    [HttpDelete]
    public IActionResult DeleteCargoCompany(int id)
    {
        _cargoCompanyService.TDelete(id);
        return Ok("Cargo company deleted successfully");
    }

    [HttpGet("{id}")]
    public IActionResult GetCargoCompanyById(int id)
    {
        var values = _cargoCompanyService.TGetById(id);
        return Ok(values);
    }

    [HttpPut]
    public IActionResult UpdateCargoCompany(UpdateCargoCompanyDto updateCargoCompanyDto)
    {
        CargoCompany updateCargoCompany = new CargoCompany
        {
            CargoCompanyId = updateCargoCompanyDto.CargoCompanyId,
            CargoCompanyName = updateCargoCompanyDto.CargoCompanyName,
        };

        _cargoCompanyService.TUpdate(updateCargoCompany);
        return Ok("Cargo company updated successfully");
    }
}
