using Accounting.AccountsPayable.Domain.Bills.Enums;
using Accounting.Domain.Common.Entities;

namespace Accounting.AccountsPayable.Domain.Bills.Entities;

public sealed class SupplierBill : TenantEntity
{
    private readonly List<SupplierBillLine> _lines = new();

    public Guid CompanyId { get; private set; }
    public Guid AccountingBookId { get; private set; }
    public Guid SupplierId { get; private set; }

    public string BillNumber { get; private set; } = string.Empty;
    public string SupplierInvoiceNumber { get; private set; } = string.Empty;

    public DateOnly InvoiceDate { get; private set; }
    public DateOnly DueDate { get; private set; }

    public string CurrencyCode { get; private set; } = string.Empty;

    public BillStatus Status { get; private set; }

    public IReadOnlyCollection<SupplierBillLine> Lines => _lines.AsReadOnly();

    public decimal Subtotal => _lines.Sum(x => x.Subtotal);
    public decimal TaxTotal => _lines.Sum(x => x.TaxAmount);
    public decimal TotalAmount => _lines.Sum(x => x.Total);

    public DateTime CreatedAtUtc { get; private set; }

    private SupplierBill() { }

    public SupplierBill(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid supplierId,
        string billNumber,
        string supplierInvoiceNumber,
        DateOnly invoiceDate,
        DateOnly dueDate,
        string currencyCode) : base(tenantId)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (accountingBookId == Guid.Empty)
            throw new ArgumentException("Accounting book ID is required.");

        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier ID is required.");

        if (string.IsNullOrWhiteSpace(billNumber))
            throw new ArgumentException("Bill number is required.");

        if (billNumber.Trim().Length > 50)
            throw new ArgumentException("Bill number cannot exceed 50 characters.");

        if (string.IsNullOrWhiteSpace(supplierInvoiceNumber))
            throw new ArgumentException("Supplier invoice number is required.");

        if (supplierInvoiceNumber.Trim().Length > 100)
            throw new ArgumentException(
                "Supplier invoice number cannot exceed 100 characters.");

        if (dueDate < invoiceDate)
            throw new ArgumentException(
                "Due date cannot be earlier than invoice date.");

        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentException("Currency code is required.");

        currencyCode = currencyCode.Trim().ToUpperInvariant();

        if (currencyCode.Length != 3)
            throw new ArgumentException(
                "Currency code must contain exactly 3 characters.");

        CompanyId = companyId;
        AccountingBookId = accountingBookId;
        SupplierId = supplierId;
        BillNumber = billNumber.Trim();
        SupplierInvoiceNumber = supplierInvoiceNumber.Trim();
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        CurrencyCode = currencyCode;

        Status = BillStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void AddLine(SupplierBillLine line)
    {
        EnsureDraft();

        ArgumentNullException.ThrowIfNull(line);

        _lines.Add(line);
    }

    public void RemoveLine(Guid lineId)
    {
        EnsureDraft();

        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line is null)
            throw new KeyNotFoundException("Bill line not found.");

        _lines.Remove(line);
    }

    public void SubmitForApproval()
    {
        EnsureDraft();

        if (_lines.Count == 0)
            throw new InvalidOperationException(
                "A bill must contain at least one line.");

        if (TotalAmount <= 0)
            throw new InvalidOperationException(
                "Bill total must be greater than zero.");

        Status = BillStatus.PendingApproval;
    }

    public void Approve()
    {
        if (Status != BillStatus.PendingApproval)
            throw new InvalidOperationException(
                "Only bills pending approval can be approved.");

        Status = BillStatus.Approved;
    }

    public void Reject()
    {
        if (Status != BillStatus.PendingApproval)
            throw new InvalidOperationException(
                "Only bills pending approval can be rejected.");

        Status = BillStatus.Rejected;
    }

    public void MarkAsPosted()
    {
        if (Status != BillStatus.Approved)
            throw new InvalidOperationException(
                "Only approved bills can be posted.");

        Status = BillStatus.Posted;
    }

    public void Void()
    {
        if (Status == BillStatus.Paid ||
            Status == BillStatus.PartiallyPaid ||
            Status == BillStatus.Voided)
        {
            throw new InvalidOperationException(
                "This bill cannot be voided.");
        }

        Status = BillStatus.Voided;
    }

    private void EnsureDraft()
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException(
                "Only draft bills can be modified.");
    }
}