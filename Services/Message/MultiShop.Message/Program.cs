
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MultiShop.Message.DataAccess.Contexts;
using MultiShop.Message.Services;
using Scalar.AspNetCore;

namespace MultiShop.Message;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
        {
            opt.Authority = builder.Configuration["IdentityServerUrl"];
            opt.Audience = "ResourceMessage";
            opt.RequireHttpsMetadata = true;
        });

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // DbContext registration (PostgreSQL)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<MultiShopMessageDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        // AutoMapper registration (v15-friendly)
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<Mapping.MessageMappingProfile>();
        });

        // Service registrations
        builder.Services.AddScoped<IMessageService, MessageService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                // This is the endpoint for the Swagger UI
                options.SwaggerEndpoint("/openapi/v1.json", "MultiShop Message API");
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
