
using Accounting.Domain.Common.Entities;
using Accounting.Domain.ChartOfAccounts.Enums;

namespace Accounting.Domain.ChartOfAccounts.Entities;

public class Account : TenantEntity
{
    public Guid ChartOfAccountsId { get; private set; }

    public Guid? ParentAccountId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public AccountType AccountType { get; private set; }

    public AccountCategory AccountCategory { get; private set; }

    public CashFlowCategory CashFlowCategory { get; private set; }

    public bool IsCashOrCashEquivalent { get; private set; }

    public bool IsPostingAllowed { get; private set; }

    public bool IsActive { get; private set; }

    public int Level { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private Account()
    {
    }

    public Account(
        Guid tenantId,
        Guid chartOfAccountsId,
        Guid? parentAccountId,
        string code,
        string name,
        AccountType accountType,
        AccountCategory accountCategory,
        CashFlowCategory cashFlowCategory = CashFlowCategory.Excluded,
        int level = 1,
        bool isPostingAllowed = false)
        : base(tenantId)
    {
        if (chartOfAccountsId == Guid.Empty)
            throw new ArgumentException("Chart ID is required.");

        if (parentAccountId == Guid.Empty)
            throw new ArgumentException("Parent account ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Account code is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name is required.");

        if (level < 1)
            throw new ArgumentOutOfRangeException(nameof(level));

        if (!Enum.IsDefined(accountType))
            throw new ArgumentOutOfRangeException(nameof(accountType));

        if (!Enum.IsDefined(accountCategory))
            throw new ArgumentOutOfRangeException(nameof(accountCategory));

        if (!Enum.IsDefined(cashFlowCategory))
            throw new ArgumentOutOfRangeException(nameof(cashFlowCategory));

        if (parentAccountId.HasValue && parentAccountId.Value == Guid.Empty)
            throw new ArgumentException(
                "Parent account ID cannot be empty.",
                nameof(parentAccountId));

        if (parentAccountId.HasValue && parentAccountId.Value == Id)
            throw new ArgumentException(
                "An account cannot be its own parent.",
                nameof(parentAccountId));

        ChartOfAccountsId = chartOfAccountsId;
        ParentAccountId = parentAccountId;
        Code = code.Trim();
        Name = name.Trim();
        AccountType = accountType;
        AccountCategory = accountCategory;
        CashFlowCategory = cashFlowCategory;
        Level = level;
        IsPostingAllowed = isPostingAllowed;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name is required.");

        Name = name.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void AllowPosting()
    {
        IsPostingAllowed = true;
    }

    public void DisallowPosting()
    {
        IsPostingAllowed = false;
    }

    public void UpdateCashFlowCategory(CashFlowCategory category)
    {
        if (!Enum.IsDefined(category))
            throw new ArgumentOutOfRangeException(nameof(category));

        CashFlowCategory = category;
    }

    public void SetCashOrCashEquivalent(bool isCashOrCashEquivalent)
{
    if (isCashOrCashEquivalent &&
        AccountType != AccountType.Asset)
    {
        throw new InvalidOperationException(
            "Only asset accounts can be classified as cash or cash equivalents.");
    }

    IsCashOrCashEquivalent = isCashOrCashEquivalent;
}
}
