
using Accounting.Domain.ChartOfAccounts.Entities;

namespace Accounting.Domain.ChartOfAccounts.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(
        Guid accountId,
        Guid tenantId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        Guid tenantId,
        string code,
        CancellationToken cancellationToken = default);

    Task<bool> HasChildrenAsync(
        Guid accountId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
