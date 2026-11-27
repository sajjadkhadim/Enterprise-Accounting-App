using Accounting.AccountsPayable.Domain.Bills.Entities;
using Accounting.AccountsPayable.Domain.Bills.Repositories;
using Accounting.AccountsPayable.Domain.PaymentTerms.Repositories;

namespace Accounting.AccountsPayable.Application.Bills.Services;

public sealed class CreateSupplierBillService
{
    private readonly ISupplierBillRepository _billRepository;
    private readonly IPaymentTermRepository _paymentTermRepository;

    public CreateSupplierBillService(
        ISupplierBillRepository billRepository,
        IPaymentTermRepository paymentTermRepository)
    {
        _billRepository = billRepository;
        _paymentTermRepository = paymentTermRepository;
    }

    public async Task<Guid> ExecuteAsync(
        Guid tenantId,
        Guid companyId,
        Guid accountingBookId,
        Guid supplierId,
        string billNumber,
        string supplierInvoiceNumber,
        DateOnly invoiceDate,
        DateOnly? dueDate,
        string currencyCode,
        IReadOnlyCollection<CreateBillLineData> lines,
        CancellationToken cancellationToken = default)
    {
        if (tenantId == Guid.Empty || companyId == Guid.Empty)
            throw new ArgumentException("Tenant and company are required.");

        if (!await _billRepository.AccountingBookExistsAsync(
            accountingBookId, tenantId, companyId, cancellationToken))
            throw new InvalidOperationException("Accounting book not found.");

        var supplier = await _billRepository.GetSupplierAsync(
            supplierId, tenantId, companyId, cancellationToken)
            ?? throw new InvalidOperationException("Supplier not found.");

        if (supplier.Status !=
            Accounting.AccountsPayable.Domain.Suppliers.Enums.SupplierStatus.Active)
            throw new InvalidOperationException("Supplier is not active.");

        if (supplier.CurrencyCode != currencyCode.Trim().ToUpperInvariant())
            throw new InvalidOperationException(
                "Bill currency must match supplier currency.");

        if (await _billRepository.BillNumberExistsAsync(
            billNumber.Trim(), tenantId, companyId,
            accountingBookId, cancellationToken))
            throw new InvalidOperationException(
                "Bill number already exists in this accounting book.");

        if (lines.Count == 0)
            throw new ArgumentException("At least one bill line is required.");

        var accountIds = lines.Select(x => x.AccountId).Distinct().ToList();

        if (!await _billRepository.AccountsAreValidAsync(
            accountIds, tenantId, companyId,
            accountingBookId, cancellationToken))
            throw new InvalidOperationException(
                "One or more accounts are invalid or not posting accounts.");

        var calculatedDueDate = dueDate ??
            invoiceDate.AddDays(supplier.PaymentTermsDays);

        if (supplier.PaymentTermId.HasValue)
        {
            var paymentTerm = await _paymentTermRepository.GetByIdAsync(
                supplier.PaymentTermId.Value,
                tenantId,
                companyId,
                cancellationToken);

            if (paymentTerm is null ||
                paymentTerm.Status !=
                    Accounting.AccountsPayable.Domain.PaymentTerms.Enums.PaymentTermStatus.Active)
                throw new InvalidOperationException(
                    "The supplier's payment term is unavailable or inactive.");

            calculatedDueDate = dueDate ??
                invoiceDate.AddDays(paymentTerm.DueDays);
        }

        var bill = new SupplierBill(
            tenantId,
            companyId,
            accountingBookId,
            supplierId,
            billNumber,
            supplierInvoiceNumber,
            invoiceDate,
            calculatedDueDate,
            currencyCode);

        foreach (var line in lines)
        {
            bill.AddLine(new SupplierBillLine(
                line.AccountId,
                line.Description,
                line.Quantity,
                line.UnitPrice,
                line.TaxRate));
        }

        await _billRepository.AddAsync(bill, cancellationToken);
        await _billRepository.SaveChangesAsync(cancellationToken);

        return bill.Id;
    }
}

public sealed record CreateBillLineData(
    Guid AccountId,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate);