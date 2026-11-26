
using Accounting.API.Contracts.Accounts;
using Accounting.Application.ChartOfAccounts.Services;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Controllers;

[ApiController]
[Authorize]
[Route("api/accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly CreateAccountService _createAccountService;
    private readonly UpdateAccountService _updateAccountService;

    public AccountsController(
        AppDbContext context,
        CreateAccountService createAccountService,
        UpdateAccountService updateAccountService)
    {
        _context = context;
        _createAccountService = createAccountService;
        _updateAccountService = updateAccountService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .Where(x => x.Id == id)
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
            .FirstOrDefaultAsync(ct);

        return account is null ? NotFound() : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateAccountRequest request,
        CancellationToken ct)
    {
        var accountId = await _createAccountService.ExecuteAsync(
            request.ChartOfAccountsId,
            request.ParentAccountId,
            request.Code,
            request.Name,
            request.AccountType,
            request.AccountCategory,
            request.Level,
            request.IsPostingAllowed,
            ct);

        var account = await _context.Accounts
            .AsNoTracking()
            .Where(x => x.Id == accountId)
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
            .FirstOrDefaultAsync(ct);

        if (account is null)
            return NotFound("Created account could not be retrieved.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = account.Id },
            account);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateAccountRequest request,
        CancellationToken ct)
    {
        var updated = await _updateAccountService.ExecuteAsync(
            id,
            request.Name,
            request.CashFlowCategory,
            request.IsCashOrCashEquivalent,
            request.IsPostingAllowed,
            request.IsActive,
            ct);

        if (!updated)
            return NotFound("Account not found.");

        var account = await _context.Accounts
            .AsNoTracking()
            .Where(x => x.Id == id)
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
            .FirstOrDefaultAsync(ct);

        return Ok(account);
    }
}
