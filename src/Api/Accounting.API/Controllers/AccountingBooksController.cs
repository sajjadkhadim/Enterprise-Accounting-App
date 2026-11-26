
using Accounting.API.Contracts.AccountingBooks;
using Accounting.Domain.GeneralLedger.Entities;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Controllers;

[ApiController]
[Authorize]
[Route("api/accounting-books")]
public sealed class AccountingBooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public AccountingBooksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var book = await _context.AccountingBooks
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.CompanyId,
                x.Name,
                x.Code,
                x.AccountingBasis,
                x.CurrencyCode,
                x.IsActive,
                x.CreatedAtUtc
            })
            .FirstOrDefaultAsync(ct);

        return book is null ? NotFound() : Ok(book);
    }

    [HttpGet("{accountingBookId:guid}/charts-of-accounts")]
    public async Task<IActionResult> GetCharts(
        Guid accountingBookId,
        CancellationToken ct)
    {
        var bookExists = await _context.AccountingBooks
            .AnyAsync(x => x.Id == accountingBookId, ct);

        if (!bookExists)
            return NotFound("Accounting book not found.");

        var charts = await _context.ChartsOfAccounts
            .AsNoTracking()
            .Where(x => x.AccountingBookId == accountingBookId)
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.AccountingBookId,
                x.Name,
                x.Code,
                x.Description,
                x.IsActive,
                x.CreatedAtUtc
            })
            .ToListAsync(ct);

        return Ok(charts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAccountingBookRequest request,
        CancellationToken ct)
    {
        var tenantIdValue = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdValue, out var tenantId) ||
            tenantId == Guid.Empty)
        {
            return Unauthorized("Valid tenant ID was not found.");
        }

        var company = await _context.Companies
            .FirstOrDefaultAsync(x => x.Id == request.CompanyId, ct);

        if (company is null)
            return NotFound("Company not found.");

        var code = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _context.AccountingBooks
            .AnyAsync(
                x => x.CompanyId == request.CompanyId &&
                     x.Code == code,
                ct);

        if (codeExists)
            return Conflict(
                "An accounting book with this code already exists for this company.");

        var book = new AccountingBook(
            tenantId,
            request.CompanyId,
            request.Name,
            code,
            request.AccountingBasis,
            request.CurrencyCode);

        await _context.AccountingBooks.AddAsync(book, ct);
        await _context.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = book.Id },
            new
            {
                book.Id,
                book.CompanyId,
                book.Name,
                book.Code,
                book.AccountingBasis,
                book.CurrencyCode,
                book.IsActive,
                book.CreatedAtUtc
            });
    }
}
