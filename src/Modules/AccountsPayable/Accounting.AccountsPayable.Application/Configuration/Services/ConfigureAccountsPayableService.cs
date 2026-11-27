
using Accounting.AccountsPayable.Domain.Configuration.Entities;
using Accounting.AccountsPayable.Domain.Configuration.Repositories;
using Accounting.AccountsPayable.Domain.Bills.Repositories;

namespace Accounting.AccountsPayable.Application.Configuration.Services;

public sealed class ConfigureAccountsPayableService
{
    private readonly IAccountsPayableSettingsRepository _settingsRepository;
    private readonly ISupplierBillRepository _billRepository;

    public ConfigureAccountsPayableService(
        IAccountsPayableSettingsRepository settingsRepository,
        ISupplierBillRepository billRepository)
    {
        _settingsRepository = settingsRepository;
        _billRepository = billRepository;
    }

    public async Task<Guid> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid payableControlAccountId,
        Guid? inputVatAccountId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || companyId == Guid.Empty)
            throw new ArgumentException("Tenant and company are required.");

        if (!await _billRepository.AccountingBookExistsAsync(
                accountingBookId, tenantId, companyId, cancellationToken))
            throw new ArgumentException(
                "Accounting book does not belong to this company.");

        if (!await _settingsRepository.AccountsAreValidAsync(
                payableControlAccountId,
                inputVatAccountId,
                tenantId,
                accountingBookId,
                cancellationToken))
            throw new ArgumentException(
                "One or more accounts are invalid, inactive, non-posting, or outside the accounting book.");

        var settings = await _settingsRepository.GetAsync(
            tenantId, companyId, accountingBookId, cancellationToken);

        if (settings is null)
        {
            settings = new AccountsPayableSettings(
                tenantId,
                companyId,
                accountingBookId,
                payableControlAccountId,
                inputVatAccountId);

            await _settingsRepository.AddAsync(settings, cancellationToken);
        }
        else
        {
            settings.UpdateAccounts(
                payableControlAccountId,
                inputVatAccountId);
        }

        await _settingsRepository.SaveChangesAsync(cancellationToken);

        return settings.Id;
    }
}
