using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.AboutDtos;
using MultiShop.WebUI.Services.CatalogServices.AboutServices;
using Newtonsoft.Json;

namespace MultiShop.WebUI.ViewComponents.LayoutViewComponents;

public class LayoutFooterViewComponent : ViewComponent
{
    private readonly IAboutService _aboutService;

    public LayoutFooterViewComponent(IAboutService aboutService)
    {
        _aboutService = aboutService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var values = await _aboutService.GetAllAsync();
        return View(values);
    }
}
