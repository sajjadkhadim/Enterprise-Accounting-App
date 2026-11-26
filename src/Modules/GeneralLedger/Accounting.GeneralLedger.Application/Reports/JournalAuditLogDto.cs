
namespace Accounting.GeneralLedger.Application.Reports;

public sealed record JournalAuditLogDto(
    Guid Id,
    Guid JournalEntryId,
    Guid UserId,
    string Action,
    string? PreviousStatus,
    string? NewStatus,
    string? Comments,
    DateTime OccurredAtUtc);
