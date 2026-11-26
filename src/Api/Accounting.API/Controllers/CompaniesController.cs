
using Accounting.API.Contracts.Companies;
using Accounting.Domain.Organization.Entities;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Controllers;

[ApiController]
[Authorize]
[Route("api/companies")]
public sealed class CompaniesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CompaniesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var companies = await _context.Companies
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Code,
                x.LegalName,
                x.CountryCode,
                x.BaseCurrencyCode,
                x.IsActive
            })
            .ToListAsync(ct);

        return Ok(companies);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var company = await _context.Companies
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Code,
                x.LegalName,
                x.TaxRegistrationNumber,
                x.CountryCode,
                x.BaseCurrencyCode,
                x.IsActive,
                x.CreatedAtUtc
            })
            .FirstOrDefaultAsync(ct);

        return company is null ? NotFound() : Ok(company);
    }

    [HttpGet("{companyId:guid}/accounting-books")]
    public async Task<IActionResult> GetAccountingBooks(
        Guid companyId,
        CancellationToken ct)
    {
        var companyExists = await _context.Companies
            .AnyAsync(x => x.Id == companyId, ct);

        if (!companyExists)
            return NotFound("Company not found.");

        var books = await _context.AccountingBooks
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.CompanyId,
                x.Name,
                x.Code,
                x.AccountingBasis,
                x.CurrencyCode,
                x.IsActive
            })
            .ToListAsync(ct);

        return Ok(books);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
    CreateCompanyRequest request,
    CancellationToken ct)
    {
        var tenantIdValue =
            User.FindFirst("tenant_id")?.Value;

        if (!Guid.TryParse(tenantIdValue, out var tenantId) ||
            tenantId == Guid.Empty)
        {
            return Unauthorized("Valid tenant ID was not found.");
        }

        var code = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _context.Companies
            .AnyAsync(x => x.Code == code, ct);

        if (codeExists)
            return Conflict("A company with this code already exists.");

        var company = new Company(
            tenantId,
            request.Name,
            code,
            request.LegalName,
            request.CountryCode,
            request.BaseCurrencyCode);

        await _context.Companies.AddAsync(company, ct);
        await _context.SaveChangesAsync(ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = company.Id },
            new
            {
                company.Id,
                company.Name,
                company.Code,
                company.LegalName,
                company.CountryCode,
                company.BaseCurrencyCode,
                company.IsActive
            });
    }
}
