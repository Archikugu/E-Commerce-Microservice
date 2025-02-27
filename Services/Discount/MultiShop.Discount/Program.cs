using MultiShop.Discount.Context;
using MultiShop.Discount.Services.DiscountServices;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<MultiShopDiscountDapperContext>();
builder.Services.AddTransient<IDiscountService, DiscountService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        // This is the endpoint for the Swagger UI
        options.SwaggerEndpoint("/openapi/v1.json", "MultiShop Discount API");
        options.RoutePrefix = string.Empty; // This will make Swagger UI available at the root (e.g., https://localhost:7212)
    });
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
