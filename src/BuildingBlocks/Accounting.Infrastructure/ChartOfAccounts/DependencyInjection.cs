
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Infrastructure.ChartOfAccountsFeature;

public static class DependencyInjection
{
    public static IServiceCollection AddChartOfAccountsInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }
}
