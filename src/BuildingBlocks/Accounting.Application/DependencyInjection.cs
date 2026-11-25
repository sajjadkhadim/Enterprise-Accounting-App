
using Accounting.Application.ChartOfAccounts;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddChartOfAccountsApplication();

        return services;
    }
}
