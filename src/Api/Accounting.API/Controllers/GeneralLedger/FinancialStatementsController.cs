
using Accounting.GeneralLedger.Application.Journals.Services;
using Accounting.GeneralLedger.Application.Reports.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers;

[ApiController]
[Authorize]
[Route("api/general-ledger/reports")]
public sealed class FinancialStatementsController : ControllerBase
{
    private readonly GetFinancialStatementService _service;

    public FinancialStatementsController(
        GetFinancialStatementService service)
    {
        _service = service;
    }

    [HttpGet("income-statement")]
    public Task<IActionResult> GetIncomeStatement(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] Guid chartOfAccountsId,
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] string currencyCode,
        CancellationToken cancellationToken)
    {
        return GetStatement(
            companyId,
            accountingBookId,
            chartOfAccountsId,
            FinancialStatementType.IncomeStatement,
            fromDate,
            toDate,
            currencyCode,
            cancellationToken);
    }

    [HttpGet("balance-sheet")]
    public Task<IActionResult> GetBalanceSheet(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] Guid chartOfAccountsId,
        [FromQuery] DateOnly asOfDate,
        [FromQuery] string currencyCode,
        CancellationToken cancellationToken)
    {
        return GetStatement(
            companyId,
            accountingBookId,
            chartOfAccountsId,
            FinancialStatementType.BalanceSheet,
            asOfDate,
            asOfDate,
            currencyCode,
            cancellationToken);
    }

    private async Task<IActionResult> GetStatement(
        Guid companyId,
        Guid accountingBookId,
        Guid chartOfAccountsId,
        FinancialStatementType statementType,
        DateOnly fromDate,
        DateOnly toDate,
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        try
        {
            var report = await _service.ExecuteAsync(
                tenantId,
                companyId,
                accountingBookId,
                chartOfAccountsId,
                statementType,
                fromDate,
                toDate,
                currencyCode,
                cancellationToken);

            return Ok(report);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
