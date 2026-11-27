using Accounting.AccountsPayable.Domain.Bills.Repositories;
using Accounting.AccountsPayable.Domain.Configuration.Repositories;
using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;
using Accounting.AccountsPayable.Domain.Suppliers.Repositories;
using Accounting.AccountsPayable.Infrastructure.Bills.Repositories;
using Accounting.AccountsPayable.Infrastructure.Configuration.Repositories;
using Accounting.AccountsPayable.Infrastructure.Configurations;
using Accounting.AccountsPayable.Infrastructure.PaymentTerms.Repositories;
using Accounting.AccountsPayable.Infrastructure.Suppliers.Repositories;
using Accounting.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.AccountsPayable.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsPayableInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IAppDbContextModelConfiguration, AccountsPayableModelConfiguration>();

        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IPaymentTermRepository, PaymentTermRepository>();
        services.AddScoped<ISupplierBillRepository, SupplierBillRepository>();

        services.AddScoped<IAccountsPayableSettingsRepository, AccountsPayableSettingsRepository>();

        return services;
    }
}