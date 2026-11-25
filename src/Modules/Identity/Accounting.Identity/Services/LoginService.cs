
using Accounting.Identity.Contracts;
using Accounting.Identity.Entities;
using Accounting.Identity.Persistence;
using Accounting.Identity.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Identity.Services;

public sealed class LoginService : ILoginService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITenantStatusService _tenantStatusService;

    public LoginService(
        UserManager<ApplicationUser> userManager,
        IdentityDbContext dbContext,
        IJwtTokenService jwtTokenService,
        ITenantStatusService tenantStatusService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _tenantStatusService = tenantStatusService;
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        var passwordValid = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);

            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        var membership = await _dbContext.TenantMemberships
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.UserId == user.Id
                     && x.TenantId == request.TenantId
                     && x.IsActive,
                cancellationToken);

        if (membership is null)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        var tenantIsActive =
            await _tenantStatusService.IsTenantActiveAsync(
                request.TenantId,
                cancellationToken);

        if (!tenantIsActive)
            throw new UnauthorizedAccessException(
                "Invalid email or password.");

        var token = _jwtTokenService.GenerateAccessToken(
            user.Id,
            user.Email!,
            membership.TenantId,
            membership.Role);

        return new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = _jwtTokenService.GetAccessTokenExpirationUtc(),
            UserId = user.Id,
            TenantId = membership.TenantId,
            Email = user.Email!,
            TenantRole = membership.Role
        };
    }
}
