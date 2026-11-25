namespace Accounting.GeneralLedger.Domain.Journals.Enums;

public enum JournalStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Posted = 4,
    Rejected = 5,
    Reversed = 6
}