using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.CommentServices;
using MultiShop.WebUI.Dtos.CommentDtos;

namespace MultiShop.WebUI.Areas.User.Controllers;

[Area("User")]
[Authorize]
public class ReviewsController : Controller
{
    private readonly ICommentService _commentService;
    public ReviewsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var email = User?.FindFirst("email")?.Value ?? string.Empty;
        var all = await _commentService.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(email))
        {
            all = all.Where(c => string.Equals(c.Email, email, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return View(all);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCommentDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.ProductId))
        {
            return RedirectToAction("ProductDetail", "ProductList", new { area = "", id = dto?.ProductId });
        }
        dto.FirstName = dto.FirstName ?? (User?.FindFirst("given_name")?.Value ?? "");
        dto.LastName = dto.LastName ?? (User?.FindFirst("family_name")?.Value ?? "");
        dto.Email = dto.Email ?? (User?.FindFirst("email")?.Value ?? "");
        dto.ImageUrl = dto.ImageUrl ?? string.Empty;
        if (dto.Rating < 1 || dto.Rating > 5) dto.Rating = 5;
        await _commentService.CreateAsync(dto);
        return RedirectToAction("ProductDetail", "ProductList", new { area = "", id = dto.ProductId });
    }
}


