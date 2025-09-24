using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;

namespace MultiShop.WebUI.Services.CatalogServices.CategoryServices;

public class CategoryService : ICategoryService
{
    private readonly HttpClient _httpClient;
    public CategoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<ResultCategoryDto>> GetAllCategoryAsync()
    {
        var responseMessage = await _httpClient.GetAsync("categories");
        
        if (!responseMessage.IsSuccessStatusCode)
        {
            var error = await responseMessage.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve categories. Status: {(int)responseMessage.StatusCode} {responseMessage.StatusCode}. Content: {error}");
        }
        
        var categories = await responseMessage.Content.ReadFromJsonAsync<List<ResultCategoryDto>>();
        return categories ?? new List<ResultCategoryDto>();
    }
    public async Task CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var response = await _httpClient.PostAsJsonAsync<CreateCategoryDto>("categories", createCategoryDto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create category. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
        }
    }
    public async Task UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
    {
        var response = await _httpClient.PutAsJsonAsync("categories", updateCategoryDto);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to update category. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
        }
    }
    public async Task DeleteCategoryAsync(string id)
    {
        var response = await _httpClient.DeleteAsync($"categories/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to delete category. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
        }
    }
    public async Task<GetByIdCategoryDto> GetByIdCategoryAsync(string id)
    {
        var response = await _httpClient.GetAsync($"categories/{id}");
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to retrieve category. Status: {(int)response.StatusCode} {response.StatusCode}. Content: {error}");
        }
        var category = await response.Content.ReadFromJsonAsync<GetByIdCategoryDto>();
        return category ?? throw new Exception("Category not found.");
    }
}
