
using Accounting.Domain.Common.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;

namespace Accounting.GeneralLedger.Domain.Journals.Entities;

public class JournalAuditLog : TenantEntity
{
    public Guid JournalEntryId { get; private set; }

    public Guid UserId { get; private set; }

    public JournalAuditAction Action { get; private set; }

    public JournalStatus? PreviousStatus { get; private set; }

    public JournalStatus? NewStatus { get; private set; }

    public string? Comments { get; private set; }

    public DateTime OccurredAtUtc { get; private set; }

    private JournalAuditLog()
    {
    }

    public JournalAuditLog(
        Guid tenantId,
        Guid journalEntryId,
        Guid userId,
        JournalAuditAction action,
        JournalStatus? previousStatus = null,
        JournalStatus? newStatus = null,
        string? comments = null)
        : base(tenantId)
    {
        if (journalEntryId == Guid.Empty)
            throw new ArgumentException("Journal entry ID is required.");

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        if (!Enum.IsDefined(action))
            throw new ArgumentOutOfRangeException(nameof(action));

        if (previousStatus.HasValue &&
            !Enum.IsDefined(previousStatus.Value))
            throw new ArgumentOutOfRangeException(nameof(previousStatus));

        if (newStatus.HasValue &&
            !Enum.IsDefined(newStatus.Value))
            throw new ArgumentOutOfRangeException(nameof(newStatus));

        JournalEntryId = journalEntryId;
        UserId = userId;
        Action = action;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Comments = string.IsNullOrWhiteSpace(comments)
            ? null
            : comments.Trim();

        OccurredAtUtc = DateTime.UtcNow;
    }
}
