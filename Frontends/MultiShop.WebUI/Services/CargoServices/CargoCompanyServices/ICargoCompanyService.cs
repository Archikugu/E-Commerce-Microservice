using MultiShop.WebUI.Dtos.CargoDtos.CargoCompanyDtos;

namespace MultiShop.WebUI.Services.CargoServices.CargoCompanyServices;

public interface ICargoCompanyService
{
    Task<List<ResultCargoCompanyDto>> GetAllAsync();
    Task<ResultCargoCompanyDto?> GetByIdAsync(int id);
    Task CreateAsync(CreateCargoCompanyDto dto);
    Task UpdateAsync(UpdateCargoCompanyDto dto);
    Task DeleteAsync(int id);
}


