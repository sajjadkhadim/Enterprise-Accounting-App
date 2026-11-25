
using Accounting.Application.Abstractions;
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.ChartOfAccounts.Enums;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.Domain.Common.Interfaces;

namespace Accounting.Application.ChartOfAccounts.Services;

public sealed class CreateAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly AccountHierarchyService _hierarchyService;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountService(
        IAccountRepository accountRepository,
        AccountHierarchyService hierarchyService,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _hierarchyService = hierarchyService;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(
        Guid chartOfAccountsId,
        Guid? parentAccountId,
        string code,
        string name,
        AccountType accountType,
        AccountCategory accountCategory,
        int level,
        bool isPostingAllowed,
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
                await _hierarchyService.ValidateNewAccountAsync(
                    tenantId,
                    chartOfAccountsId,
                    parentAccountId,
                    code,
                    ct);

                var account = new Account(
                    tenantId,
                    chartOfAccountsId,
                    parentAccountId,
                    code,
                    name,
                    accountType,
                    accountCategory,
                    level,
                    isPostingAllowed);

                await _accountRepository.AddAsync(account, ct);

                await _accountRepository.SaveChangesAsync(ct);

                return account.Id;
            },
            cancellationToken);
    }
}
