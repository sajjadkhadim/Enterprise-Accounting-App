namespace Accounting.GeneralLedger.Domain.Journals.ValueObjects;

public sealed record Money
{
    public decimal Amount { get; }
    public string CurrencyCode { get; }

    public Money(decimal amount, string currencyCode)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Money amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException(
                "Currency code is required.",
                nameof(currencyCode));

        if (currencyCode.Trim().Length != 3)
            throw new ArgumentException(
                "Currency code must contain exactly three characters.",
                nameof(currencyCode));

        Amount = decimal.Round(amount, 2);
        CurrencyCode = currencyCode.Trim().ToUpperInvariant();
    }

    public static Money Zero(string currencyCode) => new(0, currencyCode);
}