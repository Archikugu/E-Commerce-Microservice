using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CargoDtos.CargoCompanyDtos;
using MultiShop.WebUI.Services.CargoServices.CargoCompanyServices;

namespace MultiShop.WebUI.Areas.Admin.Controllers;

[Area("Admin")]
public class CargoController : Controller
{
    private readonly ICargoCompanyService _cargoCompanyService;

    public CargoController(ICargoCompanyService cargoCompanyService)
    {
        _cargoCompanyService = cargoCompanyService;
    }

    [HttpGet]
    public async Task<IActionResult> CargoCompanyList()
    {
        var values = await _cargoCompanyService.GetAllAsync();
        return View(values);
    }

    [HttpGet]
    public IActionResult CreateCargoCompany()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCargoCompany(CreateCargoCompanyDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        await _cargoCompanyService.CreateAsync(dto);
        return RedirectToAction("CargoCompanyList");
    }

    [HttpGet]
    public async Task<IActionResult> UpdateCargoCompany(int id)
    {
        var item = await _cargoCompanyService.GetByIdAsync(id);
        if (item == null)
        {
            return RedirectToAction("CargoCompanyList");
        }
        var model = new UpdateCargoCompanyDto
        {
            CargoCompanyId = item.CargoCompanyId,
            CargoCompanyName = item.CargoCompanyName
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCargoCompany(UpdateCargoCompanyDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }
        await _cargoCompanyService.UpdateAsync(dto);
        return RedirectToAction("CargoCompanyList");
    }

    [HttpGet]
    public async Task<IActionResult> DeleteCargoCompany(int id)
    {
        await _cargoCompanyService.DeleteAsync(id);
        return RedirectToAction("CargoCompanyList");
    }
}
