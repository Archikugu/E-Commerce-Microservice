using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CommentDtos;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponent
{
    public class ProductDetailReviewViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductDetailReviewViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.GetAsync($"https://localhost:7006/api/Comments/Product/{id}");
            
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var comments = JsonConvert.DeserializeObject<List<ResultCommentDto>>(jsonData);
                return View(comments);
            }

            return View(new List<ResultCommentDto>());
        }
    }
}
