namespace Accounting.GeneralLedger.Infrastructure.Common.Exceptions;

public sealed class LedgerConcurrencyException : Exception
{
    public LedgerConcurrencyException()
        : base(
            "The ledger balance was modified by another transaction. Please retry the operation.")
    {
    }
}