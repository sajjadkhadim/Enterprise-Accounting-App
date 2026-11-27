using Accounting.AccountsPayable.Application.PaymentTerms.DTOs;
using Accounting.AccountsPayable.Application.PaymentTerms.Services;
using Accounting.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.AccountsPayable;

[ApiController]
[Authorize]
[Route("api/accounts-payable/payment-terms")]
public sealed class PaymentTermsController : ControllerBase
{
    private readonly ITenantContext _tenantContext;
    private readonly ICompanyContext _companyContext;
    private readonly CreatePaymentTermService _createService;
    private readonly GetPaymentTermsService _getService;
    private readonly UpdatePaymentTermService _updateService;
    private readonly ChangePaymentTermStatusService _statusService;

    public PaymentTermsController(
        ITenantContext tenantContext,
        ICompanyContext companyContext,
        CreatePaymentTermService createService,
        GetPaymentTermsService getService,
        UpdatePaymentTermService updateService,
        ChangePaymentTermStatusService statusService)
    {
        _tenantContext = tenantContext;
        _companyContext = companyContext;
        _createService = createService;
        _getService = getService;
        _updateService = updateService;
        _statusService = statusService;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentTermResponse>> Create(
        [FromBody] CreatePaymentTermRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        var id = await _createService.ExecuteAsync(
            tenantId,
            companyId,
            request.Code,
            request.Name,
            request.DueDays,
            request.DiscountDays,
            request.DiscountPercentage,
            cancellationToken);

        var result = await _getService.ExecuteAsync(
            tenantId, companyId, cancellationToken);

        var created = result.First(x => x.Id == id);

        return CreatedAtAction(
            nameof(GetAll),
            null,
            created);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PaymentTermResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        var result = await _getService.ExecuteAsync(
            tenantId, companyId, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePaymentTermRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        await _updateService.ExecuteAsync(
            id,
            tenantId,
            companyId,
            request.Name,
            request.DueDays,
            request.DiscountDays,
            request.DiscountPercentage,
            cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/status/{operation}")]
    public async Task<IActionResult> ChangeStatus(
        Guid id,
        string operation,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        await _statusService.ExecuteAsync(
            id,
            tenantId,
            companyId,
            operation,
            cancellationToken);

        return NoContent();
    }

    private (Guid TenantId, Guid CompanyId) GetRequestContext()
    {
        if (!_tenantContext.HasTenant || !_tenantContext.TenantId.HasValue)
            throw new UnauthorizedAccessException(
                "Tenant context is not available.");

        if (!_companyContext.HasCompany || !_companyContext.CompanyId.HasValue)
            throw new InvalidOperationException(
                "Select a company using the X-Company-Id header.");

        return (
            _tenantContext.TenantId.Value,
            _companyContext.CompanyId.Value);
    }
}