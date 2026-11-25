
using Accounting.API.Contracts.GeneralLedger;
using Accounting.GeneralLedger.Application.Journals.Services;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Accounting.API.Controllers.GeneralLedger;

[ApiController]
[Route("api/general-ledger/journals")]
[Authorize]
public sealed class JournalsController : ControllerBase
{
    // Draft → PendingApproval → Approved → Posted
    private readonly GetJournalEntryService _getJournalService;
    private readonly PostJournalEntryService _postJournalService;
    private readonly CreateJournalEntryService _createJournalService;
    private readonly SubmitJournalForApprovalService _submitService;
    private readonly ApproveJournalEntryService _approveService;
    private readonly ReverseJournalEntryService _reverseService;
    private readonly GetPagedJournalEntriesService _getPagedService;
    public JournalsController(
        GetJournalEntryService getJournalService,
        PostJournalEntryService postJournalService,
        CreateJournalEntryService createJournalService,
        SubmitJournalForApprovalService submitService,
        ApproveJournalEntryService approveService,
        ReverseJournalEntryService reverseService,
        GetPagedJournalEntriesService getPagedService)
    {
        _getJournalService = getJournalService;
        _postJournalService = postJournalService;
        _createJournalService = createJournalService;
        _submitService = submitService;
        _approveService = approveService;
        _reverseService = reverseService;
        _getPagedService = getPagedService;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(
        Guid id,
        CancellationToken cancellationToken)
    {

        if (!Guid.TryParse(
            User.FindFirst("tenant_id")?.Value,
            out var tenantId))
        {
            return Unauthorized();
        }
        //var tenantId = GetTenantId();

        var journal = await _getJournalService.ExecuteAsync(
            id,
            tenantId,
            cancellationToken);

        if (journal is null)
            return NotFound();

        return Ok(new
        {
            journal.Id,
            journal.TenantId,
            journal.CompanyId,
            journal.AccountingBookId,
            journal.JournalNumber,
            journal.Description,
            journal.JournalDate,
            journal.JournalType,
            journal.JournalSource,
            journal.Status,
            journal.CreatedAtUtc,
            Lines = journal.Lines.Select(line => new
            {
                line.Id,
                line.AccountId,
                line.Description,
                Debit = line.Debit.Amount,
                Credit = line.Credit.Amount,
                CurrencyCode = line.Debit.CurrencyCode
            })
        });
    }

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantClaim = User.FindFirstValue("tenant_id");

        if (!Guid.TryParse(tenantClaim, out var tenantId))
            return Forbid();

        await _postJournalService.ExecuteAsync(
            id,
            tenantId,
            cancellationToken);

        return NoContent();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        CreateJournalEntryRequest request,
        CancellationToken cancellationToken)
    {
        var tenantClaim = User.FindFirstValue("tenant_id");

        if (!Guid.TryParse(tenantClaim, out var tenantId))
            return Forbid();

        try
        {
            var journal = new JournalEntry(
                tenantId,
                request.CompanyId,
                request.AccountingBookId,
                request.JournalNumber,
                request.Description ?? string.Empty,
                request.JournalDate,
                request.JournalType,
                request.JournalSource);

            foreach (var line in request.Lines)
            {
                journal.AddLine(
                    line.AccountId,
                    line.Description ?? string.Empty,
                    new Money(line.Debit, line.CurrencyCode),
                    new Money(line.Credit, line.CurrencyCode));
            }

            await _createJournalService.ExecuteAsync(
               journal,
               tenantId,
               cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = journal.Id },
                new { id = journal.Id });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> Submit(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = GetTenantId();
            await _submitService.ExecuteAsync(
                id,
                tenantId,
                cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var tenantId = GetTenantId();
            await _approveService.ExecuteAsync(
                id,
                tenantId,
                cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] Guid companyId,
        [FromQuery] Guid accountingBookId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] JournalStatus? status = null,
        [FromQuery] JournalType? journalType = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        try
        {
            var result = await _getPagedService.ExecuteAsync(
                tenantId,
                companyId,
                accountingBookId,
                pageNumber,
                pageSize,
                searchTerm,
                status,
                journalType,
                fromDate,
                toDate,
                cancellationToken);

            return Ok(new
            {
                items = result.Items,
                totalCount = result.TotalCount,
                pageNumber,
                pageSize,
                totalPages = (int)Math.Ceiling(
                    (double)result.TotalCount / pageSize)
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reverse")]
    public async Task<IActionResult> Reverse(
        Guid id,
        [FromBody] ReverseJournalEntryRequest request,
        CancellationToken cancellationToken)
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdClaim, out var tenantId))
            return Unauthorized();

        try
        {
            var reversalId = await _reverseService.ExecuteAsync(
                id,
                tenantId,
                request.ReversalJournalNumber,
                request.ReversalDate,
                cancellationToken);

            return CreatedAtAction(
                nameof(Get),
                new { id = reversalId },
                new
                {
                    reversalJournalEntryId = reversalId,
                    message = "Draft reversal journal created successfully."
                });
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
    private Guid GetTenantId()
    {
        var tenantIdValue = User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdValue, out var tenantId) ||
            tenantId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "A valid tenant context is required.");
        }

        return tenantId;
    }

}
