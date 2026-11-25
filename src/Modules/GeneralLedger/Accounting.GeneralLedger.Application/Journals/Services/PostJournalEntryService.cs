
using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Application.Ledger.Services;
using Accounting.GeneralLedger.Domain.Common.Interfaces;
using Accounting.GeneralLedger.Domain.FiscalPeriods.Interfaces;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class PostJournalEntryService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IJournalAccountValidator _accountValidator;
    private readonly IFiscalPeriodRepository _fiscalPeriodRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly UpdateLedgerBalancesService _updateLedgerBalancesService;

    public PostJournalEntryService(
        IJournalEntryRepository journalRepository,
        IJournalAccountValidator accountValidator,
        IFiscalPeriodRepository fiscalPeriodRepository,
        UpdateLedgerBalancesService updateLedgerBalancesService,
        IUnitOfWork unitOfWork)
    {
        _journalRepository = journalRepository;
        _accountValidator = accountValidator;
        _fiscalPeriodRepository = fiscalPeriodRepository;
        _updateLedgerBalancesService = updateLedgerBalancesService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(
        Guid journalEntryId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        if (journalEntryId == Guid.Empty)
            throw new ArgumentException("Journal entry ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                var journal = await _journalRepository.GetByIdAsync(
                    journalEntryId,
                    tenantId,
                    ct);

                if (journal is null)
                    throw new InvalidOperationException(
                        "Journal entry was not found.");

                if (journal.TenantId != tenantId)
                    throw new InvalidOperationException(
                        "Journal entry belongs to another tenant.");

                if (journal.Status != Accounting.GeneralLedger.Domain.Journals.Enums.JournalStatus.Approved)
                {
                    throw new InvalidOperationException(
                        "Only approved journal entries can be posted.");
                }

                var fiscalPeriod =
                    await _fiscalPeriodRepository.GetForDateAsync(
                        tenantId,
                        journal.CompanyId,
                        journal.AccountingBookId,
                        journal.JournalDate,
                        ct);

                if (fiscalPeriod is null ||
                    !fiscalPeriod.AllowsPosting(journal.JournalDate))
                {
                    throw new InvalidOperationException(
                        "The journal date does not belong to an open fiscal period.");
                }

                foreach (var line in journal.Lines)
                {
                    var isValid =
                        await _accountValidator.IsValidPostingAccountAsync(
                            tenantId,
                            journal.CompanyId,
                            journal.AccountingBookId,
                            line.AccountId,
                            ct);

                    if (!isValid)
                        throw new InvalidOperationException(
                            $"Account '{line.AccountId}' is not a valid posting account.");
                }

                journal.Post();

                // If this is a reversal, update the original journal
                // within the same database transaction.
                if (journal.ReversesJournalEntryId.HasValue)
                {
                    var originalJournal = await _journalRepository.GetByIdAsync(
                        journal.ReversesJournalEntryId.Value,
                        tenantId,
                        ct);

                    if (originalJournal is null)
                        throw new InvalidOperationException(
                            "The original journal for this reversal was not found.");

                    originalJournal.MarkAsReversed(journal.Id);
                }

                await _updateLedgerBalancesService.ExecuteAsync(
                    journal,
                    fiscalPeriod.Id,
                    ct);

                await _journalRepository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}
