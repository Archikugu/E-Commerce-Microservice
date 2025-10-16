
using MultiShop.SignalR.Hubs;
using MultiShop.SignalR.Services.SignalRCommentServices;
using MultiShop.SignalR.Services.SignalRMessageServices;
using Duende.IdentityModel.Client;
using MultiShop.SignalR.Services;

namespace MultiShop.SignalR;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true);
            });
        });

		builder.Services.AddSignalR();

		// Typed HttpClients for downstream services via Ocelot
		builder.Services.AddHttpClient<ISignalRCommentService, SignalRCommentService>(client =>
		{
			client.BaseAddress = new Uri("https://localhost:5000/services/comment/");
		}).AddHttpMessageHandler(() => new BearerHandler(builder.Configuration));
		builder.Services.AddHttpClient<ISignalRMessageService, SignalRMessageService>(client =>
		{
			client.BaseAddress = new Uri("https://localhost:5000/services/message/");
		}).AddHttpMessageHandler(() => new BearerHandler(builder.Configuration));

		// Bearer handler
		builder.Services.AddSingleton<BearerTokenCache>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

		app.UseCors("CorsPolicy");

        app.UseAuthorization();


		app.MapControllers();

		app.MapHub<SignalRHub>("/signalrhub");

        app.Run();
    }
}
