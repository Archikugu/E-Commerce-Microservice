using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MultiShop.WebUI.Handlers;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.CatalogServices.AboutServices;
using MultiShop.WebUI.Services.CatalogServices.BrandServices;
using MultiShop.WebUI.Services.CatalogServices.CategoryServices;
using MultiShop.WebUI.Services.CatalogServices.ContactServices;
using MultiShop.WebUI.Services.CatalogServices.FeatureSliderServices;
using MultiShop.WebUI.Services.CatalogServices.OfferDiscountServices;
using MultiShop.WebUI.Services.CatalogServices.ProductDetailServices;
using MultiShop.WebUI.Services.CatalogServices.ProductImageServices;
using MultiShop.WebUI.Services.CatalogServices.ProductServices;
using MultiShop.WebUI.Services.CatalogServices.SpecialOfferServices;
using MultiShop.WebUI.Services.DiscountServices.DiscountCouponServices;
using MultiShop.WebUI.Services.CommentServices;
using MultiShop.WebUI.Services.Concrete;
using MultiShop.WebUI.Services.BasketServices;
using MultiShop.WebUI.Services.OrderServices.OrderAddressServices;
using MultiShop.WebUI.Services.OrderServices.OrderOrderingServices;
using MultiShop.WebUI.Settings;

namespace MultiShop.WebUI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddCookie(JwtBearerDefaults.AuthenticationScheme, opt =>
        {
            opt.LoginPath = "/Login/Index";
            opt.LogoutPath = "/Login/Logout";
            opt.AccessDeniedPath = "/Login/AccessDenied";
            opt.Cookie.HttpOnly = true;
            opt.Cookie.SameSite = SameSiteMode.Strict;
            opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            opt.Cookie.Name = "MultiShopJwtCookie";
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
        {
            opt.LoginPath = "/Login/Index";
            opt.ExpireTimeSpan = TimeSpan.FromDays(5);
            opt.Cookie.Name = "MultiShopCookie";
            opt.SlidingExpiration = true;
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ILoginService, LoginService>();
        builder.Services.AddHttpClient<IIdentityService, IdentityService>();

        builder.Services.AddHttpClient();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Add Memory Cache
        builder.Services.AddMemoryCache();

        // Add Distributed Cache for Session
        builder.Services.AddDistributedMemoryCache();

        // Add Session
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = "MultiShop.Session";
            options.IdleTimeout = TimeSpan.FromHours(12);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings"));
        builder.Services.Configure<ServiceAPISettings>(builder.Configuration.GetSection("ServiceAPISettings"));

        builder.Services.AddScoped<ResourceOwnerPasswordTokenHandler>();
        builder.Services.AddScoped<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IClientCrendentialTokenService, ClientCrendentialTokenService>();

        var values = builder.Configuration.GetSection("ServiceAPISettings").Get<ServiceAPISettings>();
        builder.Services.AddHttpClient<IUserService, UserService>(opt =>
        {
            opt.BaseAddress = new Uri(values.IdentityServerUrl);
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        builder.Services.AddHttpClient<ICategoryService, CategoryService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IProductService, ProductService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<ISpecialOfferService, SpecialOfferService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IFeatureSliderService, FeatureSliderService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<Services.CatalogServices.FeatureServices.IFeatureService, Services.CatalogServices.FeatureServices.FeatureService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IOfferDiscountService, OfferDiscountService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IBrandService, BrandService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IAboutService, AboutService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IProductImageService, ProductImageService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IProductDetailService, ProductDetailService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<ICommentService, CommentService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Comment.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IContactService, ContactService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Catalog.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ClientCrendentialTokenHandler>();

        builder.Services.AddHttpClient<IBasketService, BasketService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Basket.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        // Discount service (Ocelot + token)
        builder.Services.AddHttpClient<IDiscountCouponService, DiscountCouponService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Discount.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        // Order Address service (Ocelot + token)
        builder.Services.AddHttpClient<IOrderAddressService, OrderAddressService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Order.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        // Order Ordering service (Ocelot + token)
        builder.Services.AddHttpClient<IOrderOrderingService, OrderOrderingService>(opt =>
        {
            opt.BaseAddress = new Uri($"{values.OcelotUrl.TrimEnd('/')}/{values.Order.Path.TrimStart('/')}");
        }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        // Enable Session
        app.UseSession();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapStaticAssets();

        // Admin area route registration
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        // Default route registration
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        app.Run();
    }
}
