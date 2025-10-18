using MultiShop.Images.Services;
using MultiShop.Images.Utils.ConfigOptions;
using Scalar.AspNetCore;

namespace MultiShop.Images;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.Configure<GCSConfigOptions>(builder.Configuration);
        builder.Services.AddSingleton<ICloudStorageService, CloudStorageService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                // This is the endpoint for the Swagger UI
                options.SwaggerEndpoint("/openapi/v1.json", "MultiShop Images API");
            });
            app.MapOpenApi();
            app.MapScalarApiReference();
        }


        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
