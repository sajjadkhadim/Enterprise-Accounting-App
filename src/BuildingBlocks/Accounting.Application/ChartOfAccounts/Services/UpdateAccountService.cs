
using Accounting.Application.Abstractions;
using Accounting.Domain.ChartOfAccounts.Enums;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.Domain.Common.Interfaces;

namespace Accounting.Application.ChartOfAccounts.Services;

public sealed class UpdateAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountService(
        IAccountRepository accountRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(
        Guid accountId,
        string? name = null,
        CashFlowCategory? cashFlowCategory = null,
        bool? isCashOrCashEquivalent = null,
        bool? isPostingAllowed = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.TenantId.HasValue)
        {
            throw new InvalidOperationException(
                "An authenticated tenant context is required.");
        }

        var tenantId = _tenantContext.TenantId.Value;

        return await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var account = await _accountRepository.GetByIdAsync(
                    accountId,
                    tenantId,
                    ct);

                if (account is null)
                    return false;

                if (name is not null)
                    account.Rename(name);

                if (cashFlowCategory.HasValue)
                    account.UpdateCashFlowCategory(cashFlowCategory.Value);

                if (isCashOrCashEquivalent.HasValue)
                    account.SetCashOrCashEquivalent(
                        isCashOrCashEquivalent.Value);

                if (isPostingAllowed.HasValue)
                {
                    if (isPostingAllowed.Value)
                        account.AllowPosting();
                    else
                        account.DisallowPosting();
                }

                if (isActive.HasValue)
                {
                    if (isActive.Value)
                        account.Activate();
                    else
                        account.Deactivate();
                }

                await _accountRepository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}
