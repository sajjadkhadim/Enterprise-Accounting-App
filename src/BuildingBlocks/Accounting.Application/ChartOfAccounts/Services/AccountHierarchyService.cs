
using Accounting.Domain.ChartOfAccounts.Interfaces;

namespace Accounting.Application.ChartOfAccounts.Services;

public sealed class AccountHierarchyService
{
    private readonly IAccountRepository _accountRepository;

    public AccountHierarchyService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task ValidateNewAccountAsync(
        Guid tenantId,
        Guid chartOfAccountsId,
        Guid? parentAccountId,
        string code,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (chartOfAccountsId == Guid.Empty)
            throw new ArgumentException("Chart ID is required.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Account code is required.");

        var codeExists = await _accountRepository.ExistsByCodeAsync(
            tenantId,
            code.Trim(),
            cancellationToken);

        if (codeExists)
            throw new InvalidOperationException(
                "This account code already exists in the tenant.");

        if (!parentAccountId.HasValue)
            return;

        if (parentAccountId.Value == Guid.Empty)
            throw new ArgumentException("Parent account ID cannot be empty.");

        var parent = await _accountRepository.GetByIdAsync(
            parentAccountId.Value,
            tenantId,
            cancellationToken);

        if (parent is null)
            throw new InvalidOperationException("Parent account was not found.");

        if (parent.TenantId != tenantId)
            throw new InvalidOperationException(
                "Parent account belongs to another tenant.");

        if (parent.ChartOfAccountsId != chartOfAccountsId)
            throw new InvalidOperationException(
                "Parent account belongs to another chart.");

        if (parent.IsPostingAllowed)
            throw new InvalidOperationException(
                "A posting account cannot have child accounts.");
    }
}
