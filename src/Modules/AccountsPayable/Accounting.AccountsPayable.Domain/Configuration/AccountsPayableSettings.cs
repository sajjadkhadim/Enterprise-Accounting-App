
namespace Accounting.AccountsPayable.Domain.Configuration.Entities;

public sealed class AccountsPayableSettings
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid AccountingBookId { get; private set; }

    public Guid PayableControlAccountId { get; private set; }
    public Guid? InputVatAccountId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private AccountsPayableSettings() { }

    public AccountsPayableSettings(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid payableControlAccountId,
        Guid? inputVatAccountId = null)
    {
        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant is required.");

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book is required.");

        if (payableControlAccountId == Guid.Empty)
            throw new ArgumentException("Payables control account is required.");

        if (inputVatAccountId == Guid.Empty)
            throw new ArgumentException("Input VAT account cannot be empty.");

        if (inputVatAccountId == payableControlAccountId)
            throw new ArgumentException(
                "Payables control and Input VAT accounts must be different.");

        Id = Guid.NewGuid();
        TenantId = tenantId;
        CompanyId = companyId;
        AccountingBookId = accountingBookId;
        PayableControlAccountId = payableControlAccountId;
        InputVatAccountId = inputVatAccountId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateAccounts(
        Guid payableControlAccountId,
        Guid? inputVatAccountId)
    {
        if (payableControlAccountId == Guid.Empty)
            throw new ArgumentException("Payables control account is required.");

        if (inputVatAccountId == Guid.Empty)
            throw new ArgumentException("Input VAT account cannot be empty.");

        if (inputVatAccountId == payableControlAccountId)
            throw new ArgumentException(
                "Payables control and Input VAT accounts must be different.");

        PayableControlAccountId = payableControlAccountId;
        InputVatAccountId = inputVatAccountId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
