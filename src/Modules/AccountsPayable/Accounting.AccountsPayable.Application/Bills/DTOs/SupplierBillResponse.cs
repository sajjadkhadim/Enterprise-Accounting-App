using Accounting.AccountsPayable.Domain.Bills.Entities;

namespace Accounting.AccountsPayable.Application.Bills.DTOs;

public sealed class SupplierBillLineResponse
{
    public Guid Id { get; init; }
    public Guid AccountId { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TaxRate { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal Total { get; init; }
}

public sealed class SupplierBillResponse
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public Guid AccountingBookId { get; init; }
    public Guid SupplierId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string SupplierInvoiceNumber { get; init; } = string.Empty;
    public DateOnly InvoiceDate { get; init; }
    public DateOnly DueDate { get; init; }
    public string CurrencyCode { get; init; } = string.Empty;
    public int Status { get; init; }
    public decimal Subtotal { get; init; }
    public decimal TaxTotal { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public IReadOnlyList<SupplierBillLineResponse> Lines { get; init; }
        = Array.Empty<SupplierBillLineResponse>();

    public static SupplierBillResponse FromEntity(SupplierBill bill)
    {
        return new SupplierBillResponse
        {
            Id = bill.Id,
            CompanyId = bill.CompanyId,
            AccountingBookId = bill.AccountingBookId,
            SupplierId = bill.SupplierId,
            BillNumber = bill.BillNumber,
            SupplierInvoiceNumber = bill.SupplierInvoiceNumber,
            InvoiceDate = bill.InvoiceDate,
            DueDate = bill.DueDate,
            CurrencyCode = bill.CurrencyCode,
            Status = (int)bill.Status,
            Subtotal = bill.Subtotal,
            TaxTotal = bill.TaxTotal,
            TotalAmount = bill.TotalAmount,
            CreatedAtUtc = bill.CreatedAtUtc,
            Lines = bill.Lines.Select(line => new SupplierBillLineResponse
            {
                Id = line.Id,
                AccountId = line.AccountId,
                Description = line.Description,
                Quantity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TaxRate = line.TaxRate,
                Subtotal = line.Subtotal,
                TaxAmount = line.TaxAmount,
                Total = line.Total
            }).ToList()
        };
    }
}