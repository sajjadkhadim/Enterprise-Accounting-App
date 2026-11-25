
namespace Accounting.Identity.Contracts;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }

    public Guid UserId { get; init; }

    public Guid TenantId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string TenantRole { get; init; } = string.Empty;
}
