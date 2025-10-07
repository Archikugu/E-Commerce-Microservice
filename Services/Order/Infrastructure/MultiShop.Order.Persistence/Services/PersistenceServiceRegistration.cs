using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiShop.Order.Application.Interfaces;
using MultiShop.Order.Domain.Entities;
using MultiShop.Order.Persistence.Context;
using MultiShop.Order.Persistence.Repositories;

namespace MultiShop.Order.Persistence.Services
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MultiShopOrderContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IOrderingRepository), typeof(OrderingRepository));
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            return services;
        }
    }
}