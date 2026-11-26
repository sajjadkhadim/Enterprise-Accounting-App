
using Accounting.GeneralLedger.Application.Journals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Authorize]
[Route("api/general-ledger/reports/account-ledger")]
public sealed class AccountLedgerController : ControllerBase
{
    private readonly GetAccountLedgerService _service;

    public AccountLedgerController(GetAccountLedgerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] Guid accountId,
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
            accountId,
            fromDate,
            toDate,
            currencyCode,
            cancellationToken);

        return Ok(report);
    }
}
