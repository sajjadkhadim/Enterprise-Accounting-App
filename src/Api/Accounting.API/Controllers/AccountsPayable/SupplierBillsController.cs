
using Accounting.AccountsPayable.Application.Bills.DTOs;
using Accounting.AccountsPayable.Application.Bills.Services;
using Accounting.AccountsPayable.Domain.Bills.Repositories;
using Accounting.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Accounting.API.Controllers.AccountsPayable;

[ApiController]
[Authorize]
[Route("api/accounts-payable/bills")]
public sealed class SupplierBillsController : ControllerBase
{
    private readonly CreateSupplierBillService _createService;
    private readonly ISupplierBillRepository _billRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ICompanyContext _companyContext;

    private readonly SubmitSupplierBillForApprovalService _submitService;
    private readonly ApproveSupplierBillService _approveService;
    private readonly RejectSupplierBillService _rejectService;
    private readonly PostSupplierBillService _postService;

    public SupplierBillsController(
        CreateSupplierBillService createService,
        SubmitSupplierBillForApprovalService submitService,
        ApproveSupplierBillService approveService,
        RejectSupplierBillService rejectService,
        PostSupplierBillService postService,
        ISupplierBillRepository billRepository,
        ITenantContext tenantContext,
        ICompanyContext companyContext)
    {
        _createService = createService;
        _submitService = submitService;
        _approveService = approveService;
        _rejectService = rejectService;
        _postService = postService;
        _billRepository = billRepository;
        _tenantContext = tenantContext;
        _companyContext = companyContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSupplierBillRequest request,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        try
        {
            var billId = await _createService.ExecuteAsync(
                _tenantContext.TenantId!.Value,
                _companyContext.CompanyId!.Value,
                request.AccountingBookId,
                request.SupplierId,
                request.BillNumber,
                request.SupplierInvoiceNumber,
                request.InvoiceDate,
                request.DueDate,
                request.CurrencyCode,
                request.Lines.Select(line => new CreateBillLineData(
                    line.AccountId,
                    line.Description,
                    line.Quantity,
                    line.UnitPrice,
                    line.TaxRate)).ToList(),
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = billId },
                new { id = billId, message = "Supplier bill created successfully." });
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        var bill = await _billRepository.GetByIdAsync(
            id,
            _tenantContext.TenantId!.Value,
            _companyContext.CompanyId!.Value,
            cancellationToken);

        if (bill is null)
            return NotFound();

        return Ok(SupplierBillResponse.FromEntity(bill));
    }

    [HttpPost("{id:guid}/submit")]
    public async Task<IActionResult> SubmitForApproval(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        try
        {
            await _submitService.ExecuteAsync(
                id,
                _tenantContext.TenantId!.Value,
                _companyContext.CompanyId!.Value,
                cancellationToken);

            return Ok(new { message = "Supplier bill submitted for approval." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        try
        {
            await _approveService.ExecuteAsync(
                id,
                _tenantContext.TenantId!.Value,
                _companyContext.CompanyId!.Value,
                cancellationToken);

            return Ok(new { message = "Supplier bill approved." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectSupplierBillRequest request,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        try
        {
            await _rejectService.ExecuteAsync(
                id,
                _tenantContext.TenantId!.Value,
                _companyContext.CompanyId!.Value,
                request.Reason,
                cancellationToken);

            return Ok(new { message = "Supplier bill rejected." });
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

    [HttpPost("{id:guid}/post")]
    public async Task<IActionResult> Post(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdValue, out var userId))
            return Unauthorized();

        try
        {
            await _postService.ExecuteAsync(
                id,
                _tenantContext.TenantId!.Value,
                _companyContext.CompanyId!.Value,
                userId,
                cancellationToken);

            return Ok(new
            {
                message = "Supplier bill posted successfully and General Ledger journal created."
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
}
