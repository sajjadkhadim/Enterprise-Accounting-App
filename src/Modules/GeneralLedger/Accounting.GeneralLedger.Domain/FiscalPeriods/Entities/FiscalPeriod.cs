using Accounting.Domain.Common.Entities;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Enums;

namespace Accounting.GeneralLedger.Domain.FiscalPeriods.Entities;

public class FiscalPeriod : TenantEntity
{
    public Guid CompanyId { get; private set; }
    public Guid AccountingBookId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }

    public FiscalPeriodStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    private FiscalPeriod()
    {
    }

    public FiscalPeriod(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        string name,
        DateOnly startDate,
        DateOnly endDate)
        : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book ID is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Fiscal period name is required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "Fiscal period end date cannot precede its start date.");

        CompanyId = companyId;
        AccountingBookId = accountingBookId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = FiscalPeriodStatus.Future;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public bool Contains(DateOnly date)
    {
        return date >= StartDate && date <= EndDate;
    }

    public void Open()
    {
        if (Status != FiscalPeriodStatus.Future)
            throw new InvalidOperationException(
                "Only future fiscal periods can be opened.");

        Status = FiscalPeriodStatus.Open;
    }

    public void Close()
    {
        if (Status != FiscalPeriodStatus.Open)
            throw new InvalidOperationException(
                "Only open fiscal periods can be closed.");

        Status = FiscalPeriodStatus.Closed;
    }

    public void Lock()
    {
        if (Status != FiscalPeriodStatus.Closed)
            throw new InvalidOperationException(
                "Only closed fiscal periods can be locked.");

        Status = FiscalPeriodStatus.Locked;
    }

    public void UpdateDateRange(
    DateOnly startDate,
    DateOnly endDate)
    {
        if (Status != FiscalPeriodStatus.Future)
            throw new InvalidOperationException(
                "Only future fiscal periods can be modified.");

        if (endDate < startDate)
            throw new ArgumentException(
                "Fiscal period end date cannot precede its start date.");

        StartDate = startDate;
        EndDate = endDate;
    }

    public void Reopen()
    {
        if (Status != FiscalPeriodStatus.Closed)
            throw new InvalidOperationException(
                "Only closed fiscal periods can be reopened.");

        Status = FiscalPeriodStatus.Open;
    }

    public bool AllowsPosting(DateOnly journalDate)
    {
        return Status == FiscalPeriodStatus.Open
               && Contains(journalDate);
    }

    public void Rename(string name)
    {
        if (Status != FiscalPeriodStatus.Future)
            throw new InvalidOperationException(
                "Only future fiscal periods can be renamed.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Fiscal period name is required.");

        Name = name.Trim();
    }
}