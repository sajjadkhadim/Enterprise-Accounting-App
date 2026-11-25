namespace Accounting.GeneralLedger.Domain.Common.Interfaces;

public interface IJournalAccountValidator
{
    Task<bool> IsValidPostingAccountAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid accountId,
        CancellationToken cancellationToken = default);
}