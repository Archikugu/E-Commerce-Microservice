using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.CategoryDtos;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents;

public class HomeCategoriesViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeCategoriesViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var client = _httpClientFactory.CreateClient();
        //Catalog Url 7001
        var responseMessage = await client.GetAsync("https://localhost:7001/api/Categories");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<ResultCategoryDto>>(jsonData);
            
            // Her category için ürün sayısını al
            foreach (var category in categories)
            {
                var productResponse = await client.GetAsync($"https://localhost:7001/api/Products/GetProductsWithCategoryByCategoryId/{category.CategoryId}");
                if (productResponse.IsSuccessStatusCode)
                {
                    var productJsonData = await productResponse.Content.ReadAsStringAsync();
                    var products = JsonConvert.DeserializeObject<List<ResultProductsWithCategoryDto>>(productJsonData);
                    category.ProductCount = products?.Count ?? 0;
                }
                else
                {
                    category.ProductCount = 0;
                }
            }
            
            return View(categories);
        }
        return View();
    }
}
