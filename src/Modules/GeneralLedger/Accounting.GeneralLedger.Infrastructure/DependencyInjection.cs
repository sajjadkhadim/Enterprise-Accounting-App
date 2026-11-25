
using Accounting.GeneralLedger.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;
using Accounting.GeneralLedger.Infrastructure.Accounts;
using Accounting.GeneralLedger.Infrastructure.FiscalPeriods;
using Accounting.GeneralLedger.Infrastructure.Journals;
using Accounting.GeneralLedger.Infrastructure.Ledger;
using Accounting.GeneralLedger.Infrastructure.Persistence;
using Accounting.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.GeneralLedger.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGeneralLedgerInfrastructure(
        this IServiceCollection services)
    {
        services.AddScoped<IJournalAccountValidator, JournalAccountValidator>();

        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();

        services.AddScoped<IAppDbContextModelConfiguration, GeneralLedgerModelConfiguration>();

        services.AddScoped<IFiscalPeriodRepository, FiscalPeriodRepository>();

        services.AddScoped<ILedgerAccountBalanceRepository, LedgerAccountBalanceRepository>();

        return services;
    }
}
