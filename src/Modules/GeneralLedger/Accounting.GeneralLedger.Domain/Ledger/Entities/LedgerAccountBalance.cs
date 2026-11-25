using Accounting.Domain.Common.Entities;

namespace Accounting.GeneralLedger.Domain.Ledger.Entities;

public class LedgerAccountBalance : TenantEntity
{
    public Guid CompanyId { get; private set; }
    public Guid AccountingBookId { get; private set; }
    public Guid FiscalPeriodId { get; private set; }
    public Guid AccountId { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    public decimal OpeningDebit { get; private set; }
    public decimal OpeningCredit { get; private set; }

    public decimal PeriodDebit { get; private set; }
    public decimal PeriodCredit { get; private set; }

    public decimal ClosingDebit { get; private set; }
    public decimal ClosingCredit { get; private set; }

    private LedgerAccountBalance()
    {
    }

    public LedgerAccountBalance(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid fiscalPeriodId,
        Guid accountId,
        string currencyCode)
        : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book ID is required.");

        if (fiscalPeriodId == Guid.Empty)
            throw new ArgumentException("Fiscal period ID is required.");

        if (accountId == Guid.Empty)
            throw new ArgumentException("Account ID is required.");

        if (string.IsNullOrWhiteSpace(currencyCode) ||
            currencyCode.Trim().Length != 3)
            throw new ArgumentException("A valid currency code is required.");

        CompanyId = companyId;
        AccountingBookId = accountingBookId;
        FiscalPeriodId = fiscalPeriodId;
        AccountId = accountId;
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
    }

    public void RecordMovement(decimal debit, decimal credit)
    {
        if (debit < 0 || credit < 0)
            throw new ArgumentOutOfRangeException(
                "Debit and credit amounts cannot be negative.");

        if (debit > 0 && credit > 0)
            throw new ArgumentException(
                "A movement cannot contain both debit and credit amounts.");

        if (debit == 0 && credit == 0)
            throw new ArgumentException(
                "A movement must contain a debit or credit amount.");

        PeriodDebit += debit;
        PeriodCredit += credit;

        RecalculateClosingBalance();
    }

    public void SetOpeningBalance(decimal debit, decimal credit)
    {
        if (PeriodDebit != 0 || PeriodCredit != 0)
            throw new InvalidOperationException(
                "Opening balances cannot be changed after movements have been recorded.");

        if (debit < 0 || credit < 0)
            throw new ArgumentOutOfRangeException(
                "Opening debit and credit amounts cannot be negative.");

        if (debit > 0 && credit > 0)
            throw new ArgumentException(
                "Opening balance cannot contain both debit and credit amounts.");

        OpeningDebit = debit;
        OpeningCredit = credit;

        RecalculateClosingBalance();
    }

    // public void ReverseMovement(decimal debit, decimal credit)
    // {
    //     if (debit < 0 || credit < 0)
    //         throw new ArgumentOutOfRangeException(
    //             "Debit and credit amounts cannot be negative.");

    //     if (debit > 0 && credit > 0)
    //         throw new ArgumentException(
    //             "A reversal movement cannot contain both debit and credit amounts.");

    //     if (debit == 0 && credit == 0)
    //         throw new ArgumentException(
    //             "A reversal movement must contain a debit or credit amount.");

    //     if (debit > PeriodCredit || credit > PeriodDebit)
    //         throw new InvalidOperationException(
    //             "The reversal exceeds the recorded period movements.");

    //     PeriodDebit -= credit;
    //     PeriodCredit -= debit;

    //     RecalculateClosingBalance();
    // }

    private void RecalculateClosingBalance()
    {
        var netBalance =
            OpeningDebit - OpeningCredit +
            PeriodDebit - PeriodCredit;

        ClosingDebit = Math.Max(netBalance, 0);
        ClosingCredit = Math.Max(-netBalance, 0);
    }

    public bool IsValid()
    {
        if (OpeningDebit < 0 ||
            OpeningCredit < 0 ||
            PeriodDebit < 0 ||
            PeriodCredit < 0 ||
            ClosingDebit < 0 ||
            ClosingCredit < 0)
        {
            return false;
        }

        if (OpeningDebit > 0 && OpeningCredit > 0)
            return false;

        if (ClosingDebit > 0 && ClosingCredit > 0)
            return false;

        var expectedNetBalance =
            OpeningDebit - OpeningCredit +
            PeriodDebit - PeriodCredit;

        var actualNetBalance =
            ClosingDebit - ClosingCredit;

        return expectedNetBalance == actualNetBalance;
    }
}