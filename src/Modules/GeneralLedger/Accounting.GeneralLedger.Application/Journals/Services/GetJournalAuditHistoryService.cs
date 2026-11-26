
using Accounting.GeneralLedger.Application.Reports;
using Accounting.GeneralLedger.Domain.Journals.Interfaces;

namespace Accounting.GeneralLedger.Application.Journals.Services;

public sealed class GetJournalAuditHistoryService
{
    private readonly IJournalEntryRepository _journalRepository;
    private readonly IJournalAuditLogRepository _auditRepository;

    public GetJournalAuditHistoryService(
        IJournalEntryRepository journalRepository,
        IJournalAuditLogRepository auditRepository)
    {
        _journalRepository = journalRepository;
        _auditRepository = auditRepository;
    }

    public async Task<IReadOnlyList<JournalAuditLogDto>> ExecuteAsync(
        Guid tenantId,
        Guid journalEntryId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || journalEntryId == Guid.Empty)
            throw new ArgumentException(
                "Tenant ID and journal entry ID are required.");

        var journal = await _journalRepository.GetByIdAsync(
            journalEntryId,
            tenantId,
            cancellationToken);

        if (journal is null)
            throw new KeyNotFoundException("Journal entry was not found.");

        var auditLogs = await _auditRepository.GetByJournalEntryAsync(
            tenantId,
            journalEntryId,
            cancellationToken);

        return auditLogs.Select(log => new JournalAuditLogDto(
            log.Id,
            log.JournalEntryId,
            log.UserId,
            log.Action.ToString(),
            log.PreviousStatus?.ToString(),
            log.NewStatus?.ToString(),
            log.Comments,
            log.OccurredAtUtc)).ToList();
    }
}
