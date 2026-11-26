
namespace Accounting.GeneralLedger.Domain.Journals.Enums;

public enum JournalAuditAction
{
    Created = 1,
    SubmittedForApproval = 2,
    Approved = 3,
    Rejected = 4,
    Posted = 5,
    Reversed = 6,
    DescriptionUpdated = 7,
    JournalDateUpdated = 8
}
