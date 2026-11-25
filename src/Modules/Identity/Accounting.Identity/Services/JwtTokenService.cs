using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Accounting.Identity.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(
        Guid userId,
        string email,
        Guid tenantId,
        string tenantRole)
    {
        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer is missing.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience is missing.");

        var secret = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT signing key is missing.");

        var keyBytes = Convert.FromBase64String(secret);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException(
                "JWT signing key must contain at least 32 bytes.");

        var expirationMinutes = int.TryParse(
            _configuration["Jwt:AccessTokenExpirationMinutes"],
            out var minutes) && minutes is > 0 and <= 60
                ? minutes
                : 15;

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("tenant_id", tenantId.ToString()),
            new Claim("tenant_role", tenantRole),
            new Claim(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
        };

        var signingKey = new SymmetricSecurityKey(keyBytes);

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public DateTime GetAccessTokenExpirationUtc()
    {
        var configuredValue =
            _configuration["Jwt:AccessTokenExpirationMinutes"];

        var expirationMinutes =
            int.TryParse(configuredValue, out var minutes)
            && minutes is > 0 and <= 60
                ? minutes
                : 15;

        return DateTime.UtcNow.AddMinutes(expirationMinutes);
    }

}