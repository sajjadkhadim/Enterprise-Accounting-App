
using Accounting.API.Contracts.ChartsOfAccounts;
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Controllers;

[ApiController]
[Authorize]
[Route("api/charts-of-accounts")]
public sealed class ChartsOfAccountsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChartsOfAccountsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var chart = await _context.ChartsOfAccounts
            .AsNoTracking()
            .Where(x => x.Id == id)
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
            .FirstOrDefaultAsync(ct);

        return chart is null ? NotFound() : Ok(chart);
    }

    [HttpGet("{chartId:guid}/accounts")]
    public async Task<IActionResult> GetAccounts(
        Guid chartId,
        CancellationToken ct)
    {
        var chartExists = await _context.ChartsOfAccounts
            .AnyAsync(x => x.Id == chartId, ct);

        if (!chartExists)
            return NotFound("Chart of accounts not found.");

        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(x => x.ChartOfAccountsId == chartId)
            .OrderBy(x => x.Code)
            .Select(x => new
            {
                x.Id,
                x.ChartOfAccountsId,
                x.ParentAccountId,
                x.Code,
                x.Name,
                x.AccountType,
                x.AccountCategory,
                x.CashFlowCategory,
                x.IsCashOrCashEquivalent,
                x.IsPostingAllowed,
                x.IsActive,
                x.Level,
                x.CreatedAtUtc
            })
            .ToListAsync(ct);

        return Ok(accounts);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateChartOfAccountsRequest request,
        CancellationToken ct)
    {
        var tenantIdValue = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdValue, out var tenantId) ||
            tenantId == Guid.Empty)
        {
            return Unauthorized("Valid tenant ID was not found.");
        }

        var book = await _context.AccountingBooks
            .FirstOrDefaultAsync(
                x => x.Id == request.AccountingBookId,
                ct);

        if (book is null)
            return NotFound("Accounting book not found.");

        var code = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _context.ChartsOfAccounts
            .AnyAsync(
                x => x.AccountingBookId == request.AccountingBookId &&
                     x.Code == code,
                ct);

        if (codeExists)
            return Conflict(
                "A chart with this code already exists for this accounting book.");

        var chart = new ChartOfAccounts(
            tenantId,
            request.AccountingBookId,
            request.Name,
            code,
            request.Description);

        await _context.ChartsOfAccounts.AddAsync(chart, ct);
        await _context.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = chart.Id },
            new
            {
                chart.Id,
                chart.AccountingBookId,
                chart.Name,
                chart.Code,
                chart.Description,
                chart.IsActive,
                chart.CreatedAtUtc
            });
    }
}
