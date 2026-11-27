using Accounting.AccountsPayable.Domain.PaymentTerms.Entities;
using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.AccountsPayable.Infrastructure.PaymentTerms.Repositories;

public sealed class PaymentTermRepository : IPaymentTermRepository
{
    private readonly AppDbContext _context;

    public PaymentTermRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<PaymentTerm?> GetByIdAsync(
        Guid id,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return _context.Set<PaymentTerm>()
            .FirstOrDefaultAsync(
                x => x.Id == id &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        code = code.Trim().ToUpperInvariant();

        return _context.Set<PaymentTerm>()
            .AnyAsync(
                x => x.Code == code &&
                     x.TenantId == tenantId &&
                     x.CompanyId == companyId,
                cancellationToken);
    }

    public async Task AddAsync(
        PaymentTerm paymentTerm,
        CancellationToken cancellationToken = default)
    {
        await _context.Set<PaymentTerm>()
            .AddAsync(paymentTerm, cancellationToken);
    }

    public async Task<IReadOnlyList<PaymentTerm>> GetAllAsync(
        Guid tenantId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<PaymentTerm>()
            .AsNoTracking()
            .Where(x =>
                x.TenantId == tenantId &&
                x.CompanyId == companyId)
            .OrderBy(x => x.DueDays)
            .ThenBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}