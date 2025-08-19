using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MultiShop.WebUI.Services.Abstract;
using MultiShop.WebUI.Services.Concrete;
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
            opt.Cookie.Name="MultiShopJwtCookie";
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
        {
            opt.LoginPath = "/Login/Index";
            opt.ExpireTimeSpan = TimeSpan.FromDays(5);
            opt.Cookie.Name = "MultiShopCookie";
            opt.SlidingExpiration=true;
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ILoginService, LoginService>();
        builder.Services.AddHttpClient<IIdentityService, IdentityService>();

        builder.Services.AddHttpClient();

        // Add services to the container.
        builder.Services.AddControllersWithViews();


        builder.Services.Configure<ClientSettings>(builder.Configuration.GetSection("ClientSettings")); 

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
