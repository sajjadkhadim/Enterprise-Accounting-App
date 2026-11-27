using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Accounting.Identity.Authorization;
using Accounting.Infrastructure.Tenancy;
using Accounting.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Accounting.API.Middleware;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        TenantContext tenantContext,
        CompanyContext companyContext,
        AppDbContext appDbContext,
        ITenantMembershipAuthorizationService membershipService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim =
                context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var tenantIdClaim =
                context.User.FindFirstValue("tenant_id");

            if (!Guid.TryParse(userIdClaim, out var userId) ||
                userId == Guid.Empty ||
                !Guid.TryParse(tenantIdClaim, out var tenantId) ||
                tenantId == Guid.Empty)
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                await context.Response.WriteAsync(
                    "A valid user and tenant context is required.");

                return;
            }

            var hasMembership =
                await membershipService.HasActiveMembershipAsync(
                    userId,
                    tenantId,
                    context.RequestAborted);

            if (!hasMembership)
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                await context.Response.WriteAsync(
                    "Active tenant membership is required.");

                return;
            }

            var tenantIsActive = await appDbContext.Tenants
                .IgnoreQueryFilters()
                .AsNoTracking()
                .AnyAsync(
                    tenant => tenant.Id == tenantId && tenant.IsActive,
                    context.RequestAborted);

            if (!tenantIsActive)
            {
                context.Response.StatusCode =
                    StatusCodes.Status403Forbidden;

                await context.Response.WriteAsync(
                    "The tenant is inactive.");

                return;
            }

            tenantContext.SetTenant(tenantId);

            // Resolve the selected company when the header is supplied.
            if (context.Request.Headers.TryGetValue(
                "X-Company-Id", out var companyHeader))
            {
                if (!Guid.TryParse(companyHeader.ToString(), out var companyId) ||
                    companyId == Guid.Empty)
                {
                    context.Response.StatusCode =
                        StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsync(
                        "A valid X-Company-Id header is required.");

                    return;
                }

                var companyIsValid = await appDbContext.Companies
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AnyAsync(
                        company =>
                            company.Id == companyId &&
                            company.TenantId == tenantId &&
                            company.IsActive,
                        context.RequestAborted);

                if (!companyIsValid)
                {
                    context.Response.StatusCode =
                        StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsync(
                        "The selected company is invalid, inactive, or does not belong to this tenant.");

                    return;
                }

                companyContext.SetCompany(companyId);
            }
        }

        await _next(context);
    }
}