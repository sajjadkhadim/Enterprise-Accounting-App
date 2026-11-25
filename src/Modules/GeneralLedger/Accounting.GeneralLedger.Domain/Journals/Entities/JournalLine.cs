using Accounting.Domain.Common.Entities;
using Accounting.GeneralLedger.Domain.Journals.ValueObjects;

namespace Accounting.GeneralLedger.Domain.Journals.Entities;

public class JournalLine : BaseEntity
{
    public Guid JournalEntryId { get; private set; }
    public Guid AccountId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Money Debit { get; private set; } = null!;
    public Money Credit { get; private set; } = null!;

    private JournalLine()
    {
    }

    internal JournalLine(
        Guid accountId,
        string description,
        Money debit,
        Money credit)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("Account ID is required.");

        ArgumentNullException.ThrowIfNull(debit);
        ArgumentNullException.ThrowIfNull(credit);

        if (debit.CurrencyCode != credit.CurrencyCode)
            throw new ArgumentException(
                "Debit and credit currencies must match.");

        if (debit.Amount > 0 && credit.Amount > 0)
            throw new ArgumentException(
                "A journal line cannot contain both debit and credit amounts.");

        if (debit.Amount == 0 && credit.Amount == 0)
            throw new ArgumentException(
                "A journal line must contain a debit or credit amount.");

        AccountId = accountId;
        Description = description?.Trim() ?? string.Empty;
        Debit = debit;
        Credit = credit;
    }
}