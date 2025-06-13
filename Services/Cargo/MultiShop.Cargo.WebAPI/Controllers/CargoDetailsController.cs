using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.Dtos.Dtos.CargoCustomerDtos;
using MultiShop.Cargo.Dtos.Dtos.CargoDetailDtos;
using MultiShop.Cargo.Entities.Concrete;

namespace MultiShop.Cargo.WebAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CargoDetailsController : ControllerBase
{
    private readonly ICargoDetailService _cargoDetailService;

    public CargoDetailsController(ICargoDetailService cargoDetailService)
    {
        _cargoDetailService = cargoDetailService;
    }

    [HttpGet]
    public IActionResult CargoDetailList()
    {
        var cargoDetails = _cargoDetailService.TGetAll();
        return Ok(cargoDetails);
    }

    [HttpGet("{id}")]
    public IActionResult GetCargoDetailById(int id)
    {
        var values = _cargoDetailService.TGetById(id);
        return Ok(values);
    }

    [HttpPost]
    public IActionResult CreateCargoDetail(CreateCargoDetailDto createCargoDetailDto)
    {
        CargoDetail createCargoDetail = new CargoDetail
        {
            Barcode = createCargoDetailDto.Barcode,
            CargoCompanyId = createCargoDetailDto.CargoCompanyId,
            RecieverCustomer = createCargoDetailDto.RecieverCustomer,
            SenderCustomer = createCargoDetailDto.SenderCustomer,
        };

        _cargoDetailService.TAdd(createCargoDetail);
        return Ok("Cargo detail created successfully");
    }

    [HttpDelete]
    public IActionResult DeleteCargoDetail(int id)
    {
        _cargoDetailService.TDelete(id);
        return Ok("Cargo detail deleted successfully");
    }

    [HttpPut]
    public IActionResult UpdateCargoDetail(UpdateCargoDetailDto updateCargoDetailDto)
    {
        CargoDetail updateCargoDetail = new CargoDetail
        {
            Barcode = updateCargoDetailDto.Barcode,
            CargoCompanyId = updateCargoDetailDto.CargoCompanyId,
            RecieverCustomer = updateCargoDetailDto.RecieverCustomer,
            SenderCustomer = updateCargoDetailDto.SenderCustomer,
            CargoDetailId = updateCargoDetailDto.CargoDetailId,
        };

        _cargoDetailService.TUpdate(updateCargoDetail);
        return Ok("Cargo detail updated successfully");
    }
}
