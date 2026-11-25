
using Accounting.GeneralLedger.Domain.Ledger.Entities;
using Accounting.GeneralLedger.Domain.Ledger.Interfaces;
using Accounting.GeneralLedger.Infrastructure.Common.Exceptions;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.GeneralLedger.Infrastructure.Ledger;

public sealed class LedgerAccountBalanceRepository
    : ILedgerAccountBalanceRepository
{
    private readonly AppDbContext _context;

    public LedgerAccountBalanceRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<LedgerAccountBalance?> GetAsync(
        Guid tenantId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        Guid accountId,
        string currencyCode,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<LedgerAccountBalance>()
            .FirstOrDefaultAsync(
                balance =>
                    balance.TenantId == tenantId &&
                    balance.AccountingBookId == accountingBookId &&
                    balance.FiscalPeriodId == fiscalPeriodId &&
                    balance.AccountId == accountId &&
                    balance.CurrencyCode == currencyCode,
                cancellationToken);
    }

    public async Task AddAsync(
        LedgerAccountBalance balance,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<LedgerAccountBalance>()
            .AddAsync(balance, cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new LedgerConcurrencyException();
        }
    }
}
