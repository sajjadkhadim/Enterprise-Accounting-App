using System.ComponentModel.DataAnnotations;
using Accounting.Domain.ChartOfAccounts.Enums;

namespace Accounting.API.Contracts.Accounts;

public sealed class UpdateAccountRequest
{
    [MaxLength(200)]
    public string? Name { get; init; }

    public CashFlowCategory? CashFlowCategory { get; init; }

    public bool? IsCashOrCashEquivalent { get; init; }

    public bool? IsPostingAllowed { get; init; }

    public bool? IsActive { get; init; }
}