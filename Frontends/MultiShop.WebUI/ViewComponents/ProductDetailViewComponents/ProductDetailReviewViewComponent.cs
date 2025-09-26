using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CommentDtos;
using MultiShop.WebUI.Services.CommentServices;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponent
{
    public class ProductDetailReviewViewComponent : ViewComponent
    {
        private readonly ICommentService _commentService;

        public ProductDetailReviewViewComponent(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var comments = await _commentService.GetByProductIdAsync(id);
            return View(comments ?? new List<ResultCommentDto>());
        }
    }
}
