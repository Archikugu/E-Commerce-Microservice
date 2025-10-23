using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Areas.Admin.Models;

namespace MultiShop.WebUI.Services.CatalogServices.CategoryServices;

public interface ICategoryService
{
    Task<List<ResultCategoryDto>> GetAllCategoryAsync();
    Task<PagedResult<ResultCategoryDto>> GetCategoriesPagedAsync(int pageNumber, int pageSize);
    Task CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
    Task DeleteCategoryAsync(string id);

    Task<GetByIdCategoryDto> GetByIdCategoryAsync(string id);
}
