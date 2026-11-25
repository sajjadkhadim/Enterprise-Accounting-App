namespace Accounting.Identity.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        Guid tenantId,
        string tenantRole);

    DateTime GetAccessTokenExpirationUtc();
}