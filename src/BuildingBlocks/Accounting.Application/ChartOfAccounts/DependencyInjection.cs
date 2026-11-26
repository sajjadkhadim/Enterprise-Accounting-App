
using Accounting.Application.ChartOfAccounts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Application.ChartOfAccounts;

public static class DependencyInjection
{
    public static IServiceCollection AddChartOfAccountsApplication(
        this IServiceCollection services)
    {
        services.AddScoped<AccountHierarchyService>();
        services.AddScoped<CreateAccountService>();
        services.AddScoped<UpdateAccountService>();

        return services;
    }
}
