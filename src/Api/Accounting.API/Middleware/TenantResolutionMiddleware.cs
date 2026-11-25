
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
        AppDbContext appDbContext,
        ITenantMembershipAuthorizationService membershipService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim =
                context.User.FindFirstValue(
                    JwtRegisteredClaimNames.Sub)
                ?? context.User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var tenantIdClaim = context.User.FindFirstValue("tenant_id");

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
        }

        await _next(context);
    }
}
