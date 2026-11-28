namespace Accounting.AccountsPayable.Domain.Bills.Entities;

public sealed class SupplierBillLine
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TaxRate { get; private set; }

    public decimal Subtotal => decimal.Round(
    Quantity * UnitPrice, 2);

    public decimal TaxAmount => decimal.Round(
        Subtotal * TaxRate / 100m, 2);

    public decimal Total => Subtotal + TaxAmount;

    private SupplierBillLine() { }

    public SupplierBillLine(
        Guid accountId,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal taxRate = 0)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("Account ID is required.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");

        if (description.Trim().Length > 500)
            throw new ArgumentException(
                "Description cannot exceed 500 characters.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(quantity), "Quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));

        if (taxRate < 0 || taxRate > 100)
            throw new ArgumentOutOfRangeException(nameof(taxRate));

        Id = Guid.NewGuid();
        AccountId = accountId;
        Description = description.Trim();
        Quantity = decimal.Round(quantity, 4);
        UnitPrice = decimal.Round(unitPrice, 4);
        TaxRate = decimal.Round(taxRate, 4);
    }
}