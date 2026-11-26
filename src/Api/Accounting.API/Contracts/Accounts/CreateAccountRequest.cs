using System.ComponentModel.DataAnnotations;
using Accounting.Domain.ChartOfAccounts.Enums;

namespace Accounting.API.Contracts.Accounts;

public sealed class CreateAccountRequest
{
    [Required]
    public Guid ChartOfAccountsId { get; init; }

    public Guid? ParentAccountId { get; init; }

    [Required]
    [MaxLength(50)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public AccountType AccountType { get; init; }

    [Required]
    public AccountCategory AccountCategory { get; init; }

    [Range(1, 100)]
    public int Level { get; init; } = 1;

    public bool IsPostingAllowed { get; init; } = true;
}