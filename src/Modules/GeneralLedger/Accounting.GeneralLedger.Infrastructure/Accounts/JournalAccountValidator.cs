
using Accounting.GeneralLedger.Domain.Common.Interfaces;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.GeneralLedger.Infrastructure.Accounts;

public sealed class JournalAccountValidator : IJournalAccountValidator
{
    private readonly AppDbContext _context;

    public JournalAccountValidator(AppDbContext context)
    {
        _context = context;
    }

    public Task<bool> IsValidPostingAccountAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty ||
            companyId == Guid.Empty ||
            accountingBookId == Guid.Empty ||
            accountId == Guid.Empty)
        {
            return Task.FromResult(false);
        }

        return (
            from account in _context.Accounts.AsNoTracking()
            join chart in _context.ChartsOfAccounts.AsNoTracking()
                on account.ChartOfAccountsId equals chart.Id
            join book in _context.AccountingBooks.AsNoTracking()
                on chart.AccountingBookId equals book.Id
            where
                account.Id == accountId &&
                account.TenantId == tenantId &&
                account.IsActive &&
                account.IsPostingAllowed &&
                chart.TenantId == tenantId &&
                chart.IsActive &&
                chart.AccountingBookId == accountingBookId &&
                book.TenantId == tenantId &&
                book.Id == accountingBookId &&
                book.CompanyId == companyId &&
                book.IsActive
            select account.Id
        ).AnyAsync(cancellationToken);
    }
}
