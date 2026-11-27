
using Accounting.AccountsPayable.Domain.Configuration.Entities;

namespace Accounting.AccountsPayable.Domain.Configuration.Repositories;

public interface IAccountsPayableSettingsRepository
{
    Task<AccountsPayableSettings?> GetAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default);

    Task<AccountsPayableSettings?> GetByIdAsync(
        Guid id,
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default);
        
    Task<bool> AccountsAreValidAsync(
        Guid payableControlAccountId,
        Guid? inputVatAccountId,
        Guid tenantId,
        Guid accountingBookId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AccountsPayableSettings settings,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
