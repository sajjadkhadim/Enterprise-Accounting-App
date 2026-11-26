
namespace Accounting.GeneralLedger.Application.Reports;

public sealed record AccountLedgerLineDto(
    Guid JournalEntryId,
    string JournalNumber,
    DateOnly JournalDate,
    string Description,
    decimal Debit,
    decimal Credit,
    decimal RunningBalance);
