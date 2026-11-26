
using Accounting.GeneralLedger.Application.FiscalPeriods.Services;
using Accounting.GeneralLedger.Application.Journals.Services;
using Accounting.GeneralLedger.Application.Ledger.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.GeneralLedger.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGeneralLedgerApplication(
        this IServiceCollection services)
    {
        // Journal Services
        // Draft → PendingApproval → Approved → Posted
        services.AddScoped<CreateJournalEntryService>();
        services.AddScoped<SubmitJournalForApprovalService>();
        services.AddScoped<ApproveJournalEntryService>();
        services.AddScoped<PostJournalEntryService>();
        services.AddScoped<UpdateLedgerBalancesService>();
        services.AddScoped<GetJournalEntryService>();
        services.AddScoped<GetPagedJournalEntriesService>();
        services.AddScoped<ReverseJournalEntryService>();
        services.AddScoped<GetTrialBalanceService>();
        services.AddScoped<GetAccountActivityService>();
        services.AddScoped<GetFinancialStatementService>();
        services.AddScoped<CashFlowStatementService>();
        services.AddScoped<GetAccountLedgerService>();
        services.AddScoped<GetJournalRegisterService>();
        services.AddScoped<GetJournalAuditHistoryService>();
        services.AddScoped<RejectJournalEntryService>();
        // Fiscal Period Service
        services.AddScoped<CreateFiscalPeriodService>();
        services.AddScoped<GetFiscalPeriodsService>();
        services.AddScoped<UpdateFiscalPeriodService>();
        services.AddScoped<UpdateFiscalPeriodStatusService>();
        services.AddScoped<CarryForwardFiscalPeriodBalancesService>();
        services.AddScoped<YearEndClosingService>();

        return services;
    }
}
