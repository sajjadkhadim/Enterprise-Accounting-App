

using Accounting.GeneralLedger.Application.Journals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers;

[ApiController]
[Authorize]
[Route("api/general-ledger/reports/account-activity")]
public sealed class AccountActivityController : ControllerBase
{
    private readonly GetAccountActivityService _service;

    public AccountActivityController(GetAccountActivityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] Guid fiscalPeriodId,
        [FromQuery] Guid accountId,
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
                accountId,
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
    }
}
