
using Accounting.Domain.Tenancy.Entities;
using Accounting.Identity.Entities;
using Accounting.Identity.Persistence;
using Accounting.Identity.Seeding;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Seeding;

public sealed class DevelopmentSeedService : IDevelopmentSeedService
{
    private readonly AppDbContext _appDbContext;
    private readonly IdentityDbContext _identityDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public DevelopmentSeedService(
        AppDbContext appDbContext,
        IdentityDbContext identityDbContext,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _appDbContext = appDbContext;
        _identityDbContext = identityDbContext;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        var tenantName = _configuration["Seed:TenantName"]
            ?? throw new InvalidOperationException(
                "Seed:TenantName is missing.");

        var tenantCode = _configuration["Seed:TenantCode"]
            ?? throw new InvalidOperationException(
                "Seed:TenantCode is missing.");

        var email = _configuration["Seed:UserEmail"]
            ?? throw new InvalidOperationException(
                "Seed:UserEmail is missing.");

        var password = _configuration["Seed:UserPassword"]
            ?? throw new InvalidOperationException(
                "Seed:UserPassword is missing.");

        var tenant = await _appDbContext.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.Code == tenantCode.ToUpperInvariant(),
                cancellationToken);

        if (tenant is null)
        {
            tenant = new Tenant(tenantName, tenantCode);

            await _appDbContext.Tenants.AddAsync(
                tenant, cancellationToken);

            await _appDbContext.SaveChangesAsync(
                cancellationToken);
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser(
                email,
                "Development",
                "Administrator");

            user.LockoutEnabled = true;

            var result = await _userManager.CreateAsync(
                user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(x => x.Description));

                throw new InvalidOperationException(
                    $"Development user creation failed: {errors}");
            }
        }

        var membershipExists =
            await _identityDbContext.TenantMemberships
                .AnyAsync(
                    x => x.UserId == user.Id
                         && x.TenantId == tenant.Id,
                    cancellationToken);

        if (!membershipExists)
        {
            var membership = new TenantMembership(
                user.Id,
                tenant.Id,
                "TenantAdmin");

            await _identityDbContext.TenantMemberships.AddAsync(
                membership, cancellationToken);

            await _identityDbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}
