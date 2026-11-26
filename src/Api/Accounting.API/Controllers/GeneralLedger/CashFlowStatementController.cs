
using Accounting.GeneralLedger.Application.Journals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Authorize]
[Route("api/general-ledger/reports/cash-flow-statement")]
public sealed class CashFlowStatementController : ControllerBase
{
    private readonly CashFlowStatementService _service;

    public CashFlowStatementController(CashFlowStatementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] Guid chartOfAccountsId,
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
        [FromQuery] string currencyCode,
        CancellationToken cancellationToken)
    {
        var tenantIdValue = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdValue, out var tenantId))
            return Unauthorized();

        var report = await _service.ExecuteAsync(
            tenantId,
            companyId,
            accountingBookId,
            chartOfAccountsId,
            fromDate,
            toDate,
            currencyCode,
            cancellationToken);

        return Ok(report);
    }
}
