
using COA = Accounting.Domain.ChartOfAccounts.Entities.ChartOfAccounts;

namespace Accounting.Domain.ChartOfAccounts.Interfaces;

public interface IChartOfAccountsRepository
{
    Task<COA?> GetByIdAsync(
        Guid chartOfAccountsId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<COA?> GetByAccountingBookAsync(
        Guid accountingBookId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
