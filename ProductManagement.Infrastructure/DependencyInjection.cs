using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Application.Interfaces;
using ProductManagement.Infrastructure.Persistence;
using ProductManagement.Infrastructure.Repositories;

namespace ProductManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<ProductManagementDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}
