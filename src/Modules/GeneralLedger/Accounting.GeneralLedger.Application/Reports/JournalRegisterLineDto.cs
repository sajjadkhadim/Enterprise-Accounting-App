
namespace Accounting.GeneralLedger.Application.Reports;

public sealed record JournalRegisterLineDto(
    Guid JournalEntryId,
    string JournalNumber,
    DateOnly JournalDate,
    string JournalType,
    string JournalSource,
    string Description,
    string Status,
    decimal TotalDebit,
    decimal TotalCredit);
