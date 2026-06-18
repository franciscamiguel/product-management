using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProductManagement.Infrastructure.Persistence;

namespace ProductManagement.Api.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ProductManagementDbContext>>();
            services.RemoveAll<ProductManagementDbContext>();

            services.AddDbContext<ProductManagementDbContext>(options =>
                options.UseInMemoryDatabase($"ProductManagementTests-{Guid.NewGuid()}"));
        });
    }
}
