using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.OrderDtos.OrderAddressDtos;
using MultiShop.WebUI.Services.OrderServices.OrderAddressServices;
using System.Security.Claims;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MultiShop.WebUI.Services.Abstract;

namespace MultiShop.WebUI.Controllers
{
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class OrderController : Controller
    {
        private readonly IOrderAddressService _orderAddressService;
        private readonly ILoginService _loginService;

        public OrderController(IOrderAddressService orderAddressService, ILoginService loginService)
        {
            _orderAddressService = orderAddressService;
            _loginService = loginService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new CreateOrderAddressDto();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] CreateOrderAddressDto model)
        {
            // Centralized user id resolution
            model.UserId = _loginService.GetUserId;
            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                model.UserId = User.FindFirst("sub")?.Value
                    ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? model.UserId;
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please check the form and try again.";
                return RedirectToAction("Index", "Order");
            }

            if (string.IsNullOrWhiteSpace(model.UserId))
            {
                // Fallback: access token'dan 'sub' çıkar
                var token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (!string.IsNullOrWhiteSpace(token))
                {
                    try
                    {
                        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(token);
                        var sub = jwt?.Claims?.FirstOrDefault(c => c.Type == "sub")?.Value;
                        if (!string.IsNullOrWhiteSpace(sub))
                        {
                            model.UserId = sub;
                        }
                    }
                    catch { }
                }
            }
            try
            {
                await _orderAddressService.CreateAsync(model);
                TempData["OrderMessage"] = "Address saved.";
                return RedirectToAction("Index", "Payment");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.IsNullOrWhiteSpace(ex.Message) ? "Failed to create order address." : ex.Message;
                return RedirectToAction("Index", "Order");
            }
        }
    }
}
