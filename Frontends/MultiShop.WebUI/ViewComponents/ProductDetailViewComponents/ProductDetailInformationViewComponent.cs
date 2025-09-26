using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Dtos.CatalogDtos.ProductDetailDtos;
using MultiShop.WebUI.Services.CatalogServices.ProductDetailServices;

namespace MultiShop.WebUI.ViewComponents.ProductDetailViewComponent
{
    public class ProductDetailInformationViewComponent : ViewComponent
    {
        private readonly IProductDetailService _productDetailService;
        public ProductDetailInformationViewComponent(IProductDetailService productDetailService)
        {
            _productDetailService = productDetailService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var details = await _productDetailService.GetDetailsByProductIdAsync(id);
            var productDetail = details?.FirstOrDefault();
            return View("Default", productDetail);
        }
    }
}
