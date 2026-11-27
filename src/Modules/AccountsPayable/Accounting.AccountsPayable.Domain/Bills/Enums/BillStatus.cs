namespace Accounting.AccountsPayable.Domain.Bills.Enums;

public enum BillStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    Posted = 4,
    PartiallyPaid = 5,
    Paid = 6,
    Rejected = 7,
    Voided = 8
}