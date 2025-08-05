using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.HomeViewComponents;

public class HomeFeaturedProductsViewComponent : ViewComponent
{
    private readonly IHttpClientFactory _httpClientFactory;
    public HomeFeaturedProductsViewComponent(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var client = _httpClientFactory.CreateClient();
        //Catalog Url 7001
        var responseMessage = await client.GetAsync("https://localhost:7001/api/Products");
        if (responseMessage.IsSuccessStatusCode)
        {
            var jsonData = await responseMessage.Content.ReadAsStringAsync();
            var values = JsonConvert.DeserializeObject<List<ResultProductDto>>(jsonData);
            return View(values);
        }
        return View();
    }
}
