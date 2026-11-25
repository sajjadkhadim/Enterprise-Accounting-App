
using System.ComponentModel.DataAnnotations;

namespace Accounting.Identity.Contracts;

public sealed class LoginRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MaxLength(128)]
    public string Password { get; init; } = string.Empty;

    [Required]
    public Guid TenantId { get; init; }
}
