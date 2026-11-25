
using Accounting.Application.Abstractions;
using Accounting.Domain.Common.Interfaces;
using Accounting.Infrastructure.ChartOfAccountsFeature;
using Accounting.Infrastructure.Persistence;
using Accounting.Infrastructure.Persistence.Context;
using Accounting.Infrastructure.Persistence.Interceptors;
using Accounting.Infrastructure.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Database connection string 'DefaultConnection' was not found.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            options.UseSnakeCaseNamingConvention();
        });

        services.AddScoped<TenantContext>();

        services.AddScoped<ITenantContext>(provider =>
            provider.GetRequiredService<TenantContext>());

        services.AddScoped<TenantSessionInterceptor>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddChartOfAccountsInfrastructure();

        return services;
    }
}
