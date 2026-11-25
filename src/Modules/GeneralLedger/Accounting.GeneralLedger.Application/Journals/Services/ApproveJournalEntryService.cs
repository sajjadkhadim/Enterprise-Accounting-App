using Accounting.Application.Abstractions;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class ApproveJournalEntryService
{
    private readonly IJournalEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveJournalEntryService(
        IJournalEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
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
                var journal = await _repository.GetByIdAsync(
                    journalEntryId,
                    tenantId,
                    ct);

                if (journal is null)
                    throw new KeyNotFoundException("Journal entry was not found.");

                journal.Approve();

                await _repository.SaveChangesAsync(ct);

                return true;
            },
            cancellationToken);
    }
}