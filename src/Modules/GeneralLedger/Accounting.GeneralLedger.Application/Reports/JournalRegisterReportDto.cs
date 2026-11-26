
namespace Accounting.GeneralLedger.Application.Reports;

public sealed record JournalRegisterReportDto(
    Guid CompanyId,
    Guid AccountingBookId,
    DateOnly FromDate,
    DateOnly ToDate,
    IReadOnlyList<JournalRegisterLineDto> Journals,
    decimal TotalDebit,
    decimal TotalCredit,
    int TotalJournalCount);
