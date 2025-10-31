using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CommentDtos;
using MultiShop.WebUI.Services.CommentServices;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponent
{
    public class ProductReviewsViewComponent : ViewComponent
    {
        private readonly ICommentService _commentService;

        public ProductReviewsViewComponent(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var comments = await _commentService.GetByProductIdAsync(id);
            ViewBag.ProductId = id;
            return View(comments ?? new List<ResultCommentDto>());
        }
    }
}
