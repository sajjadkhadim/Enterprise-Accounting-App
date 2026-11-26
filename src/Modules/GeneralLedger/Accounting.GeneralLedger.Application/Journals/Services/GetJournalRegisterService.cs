
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetJournalRegisterService
{
    private readonly IJournalEntryRepository _journalRepository;

    public GetJournalRegisterService(
        IJournalEntryRepository journalRepository)
    {
        _journalRepository = journalRepository;
    }

    public async Task<JournalRegisterReportDto> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty ||
            companyId == Guid.Empty ||
            accountingBookId == Guid.Empty)
        {
            throw new ArgumentException(
                "Tenant, company, and accounting book IDs are required.");
        }

        if (fromDate > toDate)
            throw new ArgumentException(
                "From date cannot be after to date.");

        var journals = await _journalRepository.GetPostedJournalsAsync(
            tenantId,
            companyId,
            accountingBookId,
            fromDate,
            toDate,
            cancellationToken);

        var lines = journals.Select(journal =>
        {
            var totalDebit = journal.Lines.Sum(
                line => line.Debit.Amount);

            var totalCredit = journal.Lines.Sum(
                line => line.Credit.Amount);

            return new JournalRegisterLineDto(
                journal.Id,
                journal.JournalNumber,
                journal.JournalDate,
                journal.JournalType.ToString(),
                journal.JournalSource.ToString(),
                journal.Description,
                journal.Status.ToString(),
                totalDebit,
                totalCredit);
        }).ToList();

        return new JournalRegisterReportDto(
            companyId,
            accountingBookId,
            fromDate,
            toDate,
            lines,
            lines.Sum(x => x.TotalDebit),
            lines.Sum(x => x.TotalCredit),
            lines.Count);
    }
}
