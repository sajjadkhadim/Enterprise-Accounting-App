
using Accounting.GeneralLedger.Application.FiscalPeriods.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[Authorize]
[ApiController]
[Route("api/general-ledger/fiscal-periods")]
public class CarryForwardController : ControllerBase
{
    private readonly CarryForwardFiscalPeriodBalancesService _service;

    public CarryForwardController(
        CarryForwardFiscalPeriodBalancesService service)
    {
        _service = service;
    }

    [HttpPost("{id:guid}/carry-forward")]
    public async Task<IActionResult> CarryForward(
        Guid id,
        CancellationToken cancellationToken)
    {
        var count = await _service.ExecuteAsync(
            id,
            cancellationToken);

        return Ok(new
        {
            message = "Opening balances carried forward successfully.",
            targetFiscalPeriodId = id,
            balancesCarriedForward = count
        });
    }
}
