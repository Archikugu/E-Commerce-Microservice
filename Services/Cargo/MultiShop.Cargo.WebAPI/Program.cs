using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MultiShop.Cargo.Business.Abstract;
using MultiShop.Cargo.Business.Concrete;
using MultiShop.Cargo.DataAccess.Abstract;
using MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Contexts;
using MultiShop.Cargo.DataAccess.Concrete.EntityFramework.Repositories;
using Scalar.AspNetCore;

namespace MultiShop.Cargo.WebAPI;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.Authority = builder.Configuration["IdentityServerUrl"];
            opt.Audience = "ResourceCargo";
            opt.RequireHttpsMetadata = true;
        });

        // Add services to the container.
        builder.Services.AddDbContext<MultiShopCargoContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<ICargoCompanyDal, EfCargoCompanyDal>();
        builder.Services.AddScoped<ICargoCompanyService, CargoCompanyManager>();

        builder.Services.AddScoped<ICargoCustomerDal, EfCargoCustomerDal>();
        builder.Services.AddScoped<ICargoCustomerService, CargoCustomerManager>();

        builder.Services.AddScoped<ICargoDetailDal, EfCargoDetailDal>();
        builder.Services.AddScoped<ICargoDetailService, CargoDetailManager>();

        builder.Services.AddScoped<ICargoOperationDal, EfCargoOperationDal>();
        builder.Services.AddScoped<ICargoOperationService, CargoOperationManager>();

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                // This is the endpoint for the Swagger UI
                options.SwaggerEndpoint("/openapi/v1.json", "MultiShop Catalog API");
            });
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
