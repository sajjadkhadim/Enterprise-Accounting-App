
using Accounting.AccountsPayable.Application.Configuration.Services;
using Accounting.AccountsPayable.Domain.Configuration.Repositories;
using Accounting.Domain.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Controllers.AccountsPayable;

[ApiController]
[Authorize]
[Route("api/accounts-payable/settings")]
public sealed class AccountsPayableSettingsController : ControllerBase
{
    private readonly ConfigureAccountsPayableService _service;
    private readonly IAccountsPayableSettingsRepository _settingsRepository;
    private readonly ITenantContext _tenantContext;
    private readonly ICompanyContext _companyContext;

    public AccountsPayableSettingsController(
        ConfigureAccountsPayableService service,
        IAccountsPayableSettingsRepository settingsRepository,
        ITenantContext tenantContext,
        ICompanyContext companyContext)
    {
        _service = service;
        _settingsRepository = settingsRepository;
        _tenantContext = tenantContext;
        _companyContext = companyContext;
    }

    [HttpPut]
    public async Task<IActionResult> Configure(
        [FromBody] ConfigureAccountsPayableRequest request,
        CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        try
        {
            var settingsId = await _service.ExecuteAsync(
                _tenantContext.TenantId!.Value,
                _companyContext.CompanyId!.Value,
                request.AccountingBookId,
                request.PayableControlAccountId,
                request.InputVatAccountId,
                cancellationToken);

            return Ok(new
            {
                id = settingsId,
                message = "Accounts Payable settings saved successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{accountingBookId:guid}")]
    public async Task<IActionResult> Get(
    Guid accountingBookId,
    CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant || !_companyContext.HasCompany)
            return BadRequest("A tenant and company must be selected.");

        var settings = await _settingsRepository.GetAsync(
            _tenantContext.TenantId!.Value,
            _companyContext.CompanyId!.Value,
            accountingBookId,
            cancellationToken);

        if (settings is null)
            return NotFound();

        return Ok(new
        {
            settings.Id,
            settings.AccountingBookId,
            settings.PayableControlAccountId,
            settings.InputVatAccountId,
            settings.CreatedAtUtc,
            settings.UpdatedAtUtc
        });
    }
}

public sealed class ConfigureAccountsPayableRequest
{
    [Required]
    public Guid AccountingBookId { get; set; }

    [Required]
    public Guid PayableControlAccountId { get; set; }

    public Guid? InputVatAccountId { get; set; }
}
