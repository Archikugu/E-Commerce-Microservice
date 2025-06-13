using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.Dtos.Dtos.CargoOperationDtos;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CargoOperationsController : ControllerBase
{
    private readonly ICargoOperationService _CargoOperationService;

    public CargoOperationsController(ICargoOperationService CargoOperationService)
    {
        _CargoOperationService = CargoOperationService;
    }

    [HttpGet]
    public IActionResult CargoOperationList()
    {
        var cargoOperations = _CargoOperationService.TGetAll();
        return Ok(cargoOperations);
    }

    [HttpPost]
    public IActionResult CreateCargoOperation(CreateCargoOperationDto createCargoOperationDto)
    {
        CargoOperation createCargoOperation = new CargoOperation
        {
            Barcode = createCargoOperationDto.Barcode,
            Description = createCargoOperationDto.Description,
            OperationDate = createCargoOperationDto.OperationDate,
        };

        _CargoOperationService.TAdd(createCargoOperation);
        return Ok("Cargo operation created successfully");
    }

    [HttpDelete]
    public IActionResult DeleteCargoOperation(int id)
    {
        _CargoOperationService.TDelete(id);
        return Ok("Cargo operation deleted successfully");
    }

    [HttpGet("{id}")]
    public IActionResult GetCargoOperationById(int id)
    {
        var values = _CargoOperationService.TGetById(id);
        return Ok(values);
    }

    [HttpPut]
    public IActionResult UpdateCargoOperation(UpdateCargoOperationDto updateCargoOperationDto)
    {
        CargoOperation updateCargoOperation = new CargoOperation
        {
            CargoOperationId = updateCargoOperationDto.CargoOperationId,
            Barcode = updateCargoOperationDto.Barcode,
            Description = updateCargoOperationDto.Description,
            OperationDate = updateCargoOperationDto.OperationDate,
        };

        _CargoOperationService.TUpdate(updateCargoOperation);
        return Ok("Cargo operation updated successfully");
    }
}
