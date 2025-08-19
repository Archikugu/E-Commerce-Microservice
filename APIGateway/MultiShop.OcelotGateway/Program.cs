using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace MultiShop.OcelotGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var AuthenticationProviderKey = "OcelotAuthenticationScheme";      
        
        // Authentication & Authorization

        builder.Services.AddAuthentication()
            .AddJwtBearer(AuthenticationProviderKey, opt =>
            {
                opt.Authority = builder.Configuration["IdentityServerUrl"];
                opt.Audience = "ResourceOcelot";
                opt.RequireHttpsMetadata = true;
            });


        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("ocelot.json").Build();

        builder.Services.AddOcelot(configuration);

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseOcelot();

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}
