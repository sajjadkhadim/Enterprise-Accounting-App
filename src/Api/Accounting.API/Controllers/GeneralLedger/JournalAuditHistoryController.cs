
using Accounting.GeneralLedger.Application.Journals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Authorize]
[Route("api/general-ledger/journals/{journalEntryId:guid}/audit-history")]
public sealed class JournalAuditHistoryController : ControllerBase
{
    private readonly GetJournalAuditHistoryService _service;

    public JournalAuditHistoryController(
        GetJournalAuditHistoryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        Guid journalEntryId,
        CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        try
        {
            var history = await _service.ExecuteAsync(
                tenantId,
                journalEntryId,
                cancellationToken);

            return Ok(history);
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
