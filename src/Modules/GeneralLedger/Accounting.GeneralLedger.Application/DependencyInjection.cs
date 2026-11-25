
using Accounting.GeneralLedger.Application.Journals.Services;
using Accounting.GeneralLedger.Application.Ledger.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.GeneralLedger.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddGeneralLedgerApplication(
        this IServiceCollection services)
    {
        // Draft → PendingApproval → Approved → Posted

        services.AddScoped<CreateJournalEntryService>();
        services.AddScoped<SubmitJournalForApprovalService>();
        services.AddScoped<ApproveJournalEntryService>();
        services.AddScoped<PostJournalEntryService>();
        services.AddScoped<UpdateLedgerBalancesService>();
        services.AddScoped<GetJournalEntryService>();

        return services;
    }
}
