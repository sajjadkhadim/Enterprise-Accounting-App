using Accounting.API.Contracts.GeneralLedger;
using Accounting.GeneralLedger.Application.FiscalPeriods.Services;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Authorize]
[Route("api/general-ledger/fiscal-periods")]
public sealed class FiscalPeriodsController : ControllerBase
{
    private readonly CreateFiscalPeriodService _createFiscalPeriodService;
    private readonly GetFiscalPeriodsService _getFiscalPeriodsService;
    private readonly UpdateFiscalPeriodService _updateFiscalPeriodService;
    private readonly UpdateFiscalPeriodStatusService _updateFiscalPeriodStatusService;

    public FiscalPeriodsController(
        CreateFiscalPeriodService createFiscalPeriodService,
        GetFiscalPeriodsService getFiscalPeriodsService,
        UpdateFiscalPeriodService updateFiscalPeriodService,
        UpdateFiscalPeriodStatusService updateFiscalPeriodStatusService)
    {
        _createFiscalPeriodService = createFiscalPeriodService;
        _getFiscalPeriodsService = getFiscalPeriodsService;
        _updateFiscalPeriodService = updateFiscalPeriodService;
        _updateFiscalPeriodStatusService = updateFiscalPeriodStatusService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateFiscalPeriodRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirst("tenant_id")?.Value,
            out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            var fiscalPeriod = new FiscalPeriod(
                tenantId,
                request.CompanyId,
                request.AccountingBookId,
                request.Name,
                request.StartDate,
                request.EndDate);

            await _createFiscalPeriodService.ExecuteAsync(
                fiscalPeriod,
                tenantId,
                cancellationToken);

            return Created(
                $"/api/general-ledger/fiscal-periods/{fiscalPeriod.Id}",
                new
                {
                    fiscalPeriod.Id,
                    fiscalPeriod.CompanyId,
                    fiscalPeriod.AccountingBookId,
                    fiscalPeriod.Name,
                    fiscalPeriod.StartDate,
                    fiscalPeriod.EndDate,
                    fiscalPeriod.Status
                });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirst("tenant_id")?.Value,
            out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            var periods = await _getFiscalPeriodsService.ExecuteAsync(
                tenantId,
                companyId,
                accountingBookId,
                cancellationToken);

            return Ok(periods.Select(period => new
            {
                period.Id,
                period.CompanyId,
                period.AccountingBookId,
                period.Name,
                period.StartDate,
                period.EndDate,
                period.Status,
                period.CreatedAtUtc
            }));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("{id:guid}/actions/{operation}")]
    public async Task<IActionResult> UpdateStatus(
    Guid id,
    string operation,
    CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirst("tenant_id")?.Value,
            out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            await _updateFiscalPeriodStatusService.ExecuteAsync(
                id,
                tenantId,
                operation,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateFiscalPeriodRequest request,
    CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            User.FindFirst("tenant_id")?.Value,
            out var tenantId))
        {
            return Unauthorized();
        }

        try
        {
            await _updateFiscalPeriodService.ExecuteAsync(
                id,
                tenantId,
                request.Name,
                request.StartDate,
                request.EndDate,
                cancellationToken);

            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}