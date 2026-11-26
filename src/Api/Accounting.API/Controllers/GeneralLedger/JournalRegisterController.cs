
using Accounting.GeneralLedger.Application.Journals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Authorize]
[Route("api/general-ledger/reports/journal-register")]
public sealed class JournalRegisterController : ControllerBase
{
    private readonly GetJournalRegisterService _service;

    public JournalRegisterController(GetJournalRegisterService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] DateOnly fromDate,
        [FromQuery] DateOnly toDate,
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
                fromDate,
                toDate,
                cancellationToken);

            return Ok(report);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
