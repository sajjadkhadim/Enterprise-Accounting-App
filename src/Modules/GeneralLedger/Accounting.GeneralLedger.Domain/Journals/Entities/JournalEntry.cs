using Accounting.Domain.Common.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.ValueObjects;

namespace Accounting.GeneralLedger.Domain.Journals.Entities;

public class JournalEntry : TenantEntity
{
    private readonly List<JournalLine> _lines = [];

    public Guid CompanyId { get; private set; }
    public Guid AccountingBookId { get; private set; }
    public Guid? ReversesJournalEntryId { get; private set; }
    public Guid? ReversedByJournalEntryId { get; private set; }
    public string JournalNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public DateOnly JournalDate { get; private set; }

    public JournalType JournalType { get; private set; }
    public JournalSource JournalSource { get; private set; }
    public JournalStatus Status { get; private set; }

    public IReadOnlyCollection<JournalLine> Lines => _lines.AsReadOnly();

    public int LineCount => _lines.Count;
    public DateTime CreatedAtUtc { get; private set; }

    private JournalEntry()
    {
    }

    public JournalEntry(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        string journalNumber,
        string description,
        DateOnly journalDate,
        JournalType journalType,
        JournalSource journalSource)
        : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book ID is required.");

        if (string.IsNullOrWhiteSpace(journalNumber))
            throw new ArgumentException("Journal number is required.");

        if (!Enum.IsDefined(journalType))
            throw new ArgumentOutOfRangeException(nameof(journalType));

        if (!Enum.IsDefined(journalSource))
            throw new ArgumentOutOfRangeException(nameof(journalSource));

        CompanyId = companyId;
        AccountingBookId = accountingBookId;
        JournalNumber = journalNumber.Trim();
        Description = description?.Trim() ?? string.Empty;
        JournalDate = journalDate;
        JournalType = journalType;
        JournalSource = journalSource;
        Status = JournalStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddLine(
        Guid accountId,
        string description,
        Money debit,
        Money credit)
    {
        if (Status != JournalStatus.Draft)
            throw new InvalidOperationException(
                "Lines can only be added to draft journals.");

        var line = new JournalLine(
            accountId,
            description,
            debit,
            credit);

        if (_lines.Count > 0)
        {
            var currency = _lines[0].Debit.CurrencyCode;

            if (line.Debit.CurrencyCode != currency)
                throw new InvalidOperationException(
                    "All journal lines must use the same currency.");
        }

        _lines.Add(line);
    }

    public bool IsBalanced()
    {
        if (_lines.Count < 2)
            return false;

        return GetTotalDebits() == GetTotalCredits();
    }

    public void SubmitForApproval()
    {
        if (Status != JournalStatus.Draft)
            throw new InvalidOperationException(
                "Only draft journals can be submitted for approval.");

        if (!IsBalanced())
            throw new InvalidOperationException(
                "Journal entry must be balanced before submission.");

        Status = JournalStatus.PendingApproval;
    }

    public void Approve()
    {
        if (Status != JournalStatus.PendingApproval)
            throw new InvalidOperationException(
                "Only journals pending approval can be approved.");

        if (!IsBalanced())
            throw new InvalidOperationException(
                "Journal entry must be balanced before approval.");

        Status = JournalStatus.Approved;
    }

    public void Reject()
    {
        if (Status != JournalStatus.PendingApproval)
            throw new InvalidOperationException(
                "Only journals pending approval can be rejected.");

        Status = JournalStatus.Rejected;
    }

    public void Post()
    {
        if (Status != JournalStatus.Approved)
            throw new InvalidOperationException(
                "Only approved journals can be posted.");

        if (!IsBalanced())
            throw new InvalidOperationException(
                "Journal entry must be balanced before posting.");

        Status = JournalStatus.Posted;
    }

    public void MarkAsReversed()
    {
        if (Status != JournalStatus.Posted)
            throw new InvalidOperationException(
                "Only posted journals can be reversed.");

        Status = JournalStatus.Reversed;
    }

    public decimal GetTotalDebits()
    {
        return _lines.Sum(line => line.Debit.Amount);
    }

    public decimal GetTotalCredits()
    {
        return _lines.Sum(line => line.Credit.Amount);
    }

    public void UpdateDescription(string description)
    {
        if (Status != JournalStatus.Draft)
            throw new InvalidOperationException(
                "Only draft journals can be modified.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(
                "Journal description is required.");

        Description = description.Trim();
    }

    public void UpdateJournalDate(DateOnly journalDate)
    {
        if (Status != JournalStatus.Draft)
            throw new InvalidOperationException(
                "Only draft journals can be modified.");

        JournalDate = journalDate;
    }
}