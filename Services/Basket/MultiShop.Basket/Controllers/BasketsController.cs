using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiShop.Basket.Dtos;
using MultiShop.Basket.LoginServices;
using MultiShop.Basket.Services;

namespace MultiShop.Basket.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketsController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly ILoginService _loginService;
    public BasketsController(IBasketService basketService, ILoginService loginService)
    {
        _basketService = basketService;
        _loginService = loginService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyBasketDetail()
    {
        //var values = await _basketService.GetBasket(_loginService.GetUserId);
        var user = User.Claims;

        var userId = _loginService.GetUserId;
        var basket = await _basketService.GetBasket(userId);
        return Ok(basket);
    }

    [HttpPost]
    public async Task<IActionResult> SaveMyBasket(BasketTotalDto basketTotalDto)
    {
        basketTotalDto.UserId = _loginService.GetUserId;
        await _basketService.SaveBasket(basketTotalDto);
        return Ok("Basket changes saved successfully.");
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMyBasket()
    {
        var userId = _loginService.GetUserId;
        await _basketService.DeleteBasket(userId);
        return Ok("Your basket has been deleted successfully.");
    }
}
