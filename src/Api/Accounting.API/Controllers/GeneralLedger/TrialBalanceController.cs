using Accounting.GeneralLedger.Application.Journals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Authorize]
[Route("api/general-ledger/reports/trial-balance")]
public sealed class TrialBalanceController : ControllerBase
{
    private readonly GetTrialBalanceService _service;

    public TrialBalanceController(GetTrialBalanceService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] Guid fiscalPeriodId,
        [FromQuery] string currencyCode,
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
                fiscalPeriodId,
                currencyCode,
                cancellationToken);

            return Ok(report);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}