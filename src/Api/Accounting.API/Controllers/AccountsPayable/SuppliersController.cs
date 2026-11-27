using Accounting.AccountsPayable.Application.Suppliers.DTOs;
using Accounting.AccountsPayable.Application.Suppliers.Services;
using Accounting.AccountsPayable.Domain.Suppliers.Enums;
using Accounting.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.API.Controllers.AccountsPayable;

[ApiController]
[Authorize]
[Route("api/accounts-payable/suppliers")]
[Produces("application/json")]
public sealed class SuppliersController : ControllerBase
{
    private readonly ITenantContext _tenantContext;
    private readonly ICompanyContext _companyContext;
    private readonly CreateSupplierService _createService;
    private readonly GetSupplierService _getService;
    private readonly UpdateSupplierService _updateService;
    private readonly UpdateSupplierFinancialDetailsService _financialService;
    private readonly ChangeSupplierStatusService _statusService;
    private readonly SearchSuppliersService _searchService;

    public SuppliersController(
        ITenantContext tenantContext,
        ICompanyContext companyContext,
        CreateSupplierService createService,
        GetSupplierService getService,
        UpdateSupplierService updateService,
        UpdateSupplierFinancialDetailsService financialService,
        ChangeSupplierStatusService statusService,
        SearchSuppliersService searchService)
    {
        _tenantContext = tenantContext;
        _companyContext = companyContext;
        _createService = createService;
        _getService = getService;
        _updateService = updateService;
        _financialService = financialService;
        _statusService = statusService;
        _searchService = searchService;
    }

    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> Create(
        [FromBody] CreateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        var supplierId = await _createService.ExecuteAsync(
            tenantId,
            companyId,
            request.SupplierCode,
            request.Name,
            request.CurrencyCode,
            request.PaymentTermsDays,
            request.CreditLimit,
            request.LegalName,
            request.TaxRegistrationNumber,
            request.Email,
            request.Phone,
            request.Address,
            request.CountryCode,
            cancellationToken);

        var supplier = await _getService.ExecuteAsync(
            supplierId, tenantId, companyId, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = supplierId },
            SupplierResponse.FromEntity(supplier));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        var supplier = await _getService.ExecuteAsync(
            id, tenantId, companyId, cancellationToken);

        return Ok(SupplierResponse.FromEntity(supplier));
    }

    [HttpGet]
    public async Task<ActionResult<SupplierListResponse>> Search(
        [FromQuery] string? search,
        [FromQuery] SupplierStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (tenantId, companyId) = GetRequestContext();

        var result = await _searchService.ExecuteAsync(
            tenantId,
            companyId,
            search,
            status,
            page,
            pageSize,
            cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSupplierRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        await _updateService.ExecuteAsync(
            id,
            tenantId,
            companyId,
            request.Name,
            request.LegalName,
            request.TaxRegistrationNumber,
            request.Email,
            request.Phone,
            request.Address,
            request.CountryCode,
            cancellationToken);

        return NoContent();
    }

    [HttpPut("{id:guid}/financial-details")]
    public async Task<IActionResult> UpdateFinancialDetails(
        Guid id,
        [FromBody] UpdateSupplierFinancialDetailsRequest request,
        CancellationToken cancellationToken)
    {
        var (tenantId, companyId) = GetRequestContext();

        await _financialService.ExecuteAsync(
            id,
            tenantId,
            companyId,
            request.CurrencyCode,
            request.PaymentTermsDays,
            request.CreditLimit,
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
            id, tenantId, companyId, operation, cancellationToken);

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

    private Guid GetCompanyIdFromHeader()
    {
        var headerValue = Request.Headers["X-Company-Id"].ToString();

        if (!Guid.TryParse(headerValue, out var companyId) ||
            companyId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "A valid X-Company-Id header is required.");
        }

        return companyId;
    }
}