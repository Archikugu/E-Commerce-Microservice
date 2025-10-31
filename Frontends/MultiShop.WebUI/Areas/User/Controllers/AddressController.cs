using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.OrderDtos.OrderAddressDtos;
using MultiShop.WebUI.Services.OrderServices.OrderAddressServices;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class AddressController : Controller
{
    private readonly IOrderAddressService _addressService;

    public AddressController(IOrderAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var list = await _addressService.GetAllAsync();
        // Ideally filter by current userId
        var userId = User?.FindFirst("sub")?.Value;
        if (!string.IsNullOrWhiteSpace(userId))
        {
            list = list.Where(x => string.Equals(x.UserId, userId, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateOrderAddressDto
        {
            UserId = User?.FindFirst("sub")?.Value
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderAddressDto dto)
    {
        dto.UserId = dto.UserId ?? User?.FindFirst("sub")?.Value;
        await _addressService.CreateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var read = await _addressService.GetByIdAsync(id);
        var model = new UpdateOrderAddressDto
        {
            AddressId = read.AddressId,
            UserId = read.UserId,
            FirstName = read.FirstName,
            LastName = read.LastName,
            Email = read.Email,
            PhoneNumber = read.PhoneNumber,
            Country = read.Country,
            Description = read.Description,
            ZipCode = read.ZipCode,
            City = read.City,
            District = read.District,
            AddressLine1 = read.AddressLine1,
            AddressLine2 = read.AddressLine2
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateOrderAddressDto dto)
    {
        await _addressService.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        await _addressService.DeleteAsync(id);
        return RedirectToAction("Index");
    }
}


