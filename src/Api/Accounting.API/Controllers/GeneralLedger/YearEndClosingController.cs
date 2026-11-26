
using System.Security.Claims;
using Accounting.API.Contracts.GeneralLedger;
using Accounting.GeneralLedger.Application.FiscalPeriods.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[Authorize]
[ApiController]
[Route("api/general-ledger/fiscal-periods")]
public class YearEndClosingController : ControllerBase
{
    private readonly YearEndClosingService _service;

    public YearEndClosingController(YearEndClosingService service)
    {
        _service = service;
    }

    [HttpPost("{id:guid}/year-end-close")]
    public async Task<IActionResult> CloseYear(
        Guid id,
        [FromBody] YearEndClosingRequest request,
        CancellationToken cancellationToken)
    {
        var userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized("Authenticated user ID is missing.");

        var journalIds = await _service.ExecuteAsync(
            id,
            request.RetainedEarningsAccountId,
            userId,
            cancellationToken);

        return Ok(new
        {
            message = "Year-end closing journals generated successfully.",
            fiscalPeriodId = id,
            journalCount = journalIds.Count,
            journalEntryIds = journalIds,
            status = "Draft"
        });
    }
}

