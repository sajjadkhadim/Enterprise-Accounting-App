
using Accounting.AccountsPayable.Domain.Bills.Entities;
using Accounting.AccountsPayable.Domain.Bills.Enums;
using Accounting.AccountsPayable.Domain.Bills.Repositories;
using Accounting.AccountsPayable.Domain.Configuration.Repositories;
using Accounting.Application.Abstractions;
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.ChartOfAccounts.Enums;
using Accounting.Domain.ChartOfAccounts.Interfaces;
using Accounting.GeneralLedger.Application.Journals.Services;
using Accounting.GeneralLedger.Domain.Journals.Entities;
using Accounting.GeneralLedger.Domain.Journals.Enums;
using Accounting.GeneralLedger.Domain.Journals.ValueObjects;

namespace Accounting.AccountsPayable.Application.Bills.Services;

public sealed class PostSupplierBillService
{
    private readonly ISupplierBillRepository _billRepository;
    private readonly IAccountsPayableSettingsRepository _settingsRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    private readonly CreateJournalEntryService _createJournalEntryService;
    private readonly SubmitJournalForApprovalService _submitJournalService;
    private readonly ApproveJournalEntryService _approveJournalService;
    private readonly PostJournalEntryService _postJournalService;

    public PostSupplierBillService(
        ISupplierBillRepository billRepository,
        IAccountsPayableSettingsRepository settingsRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork,
        CreateJournalEntryService createJournalEntryService,
        SubmitJournalForApprovalService submitJournalService,
        ApproveJournalEntryService approveJournalService,
        PostJournalEntryService postJournalService)
    {
        _billRepository = billRepository;
        _settingsRepository = settingsRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
        _createJournalEntryService = createJournalEntryService;
        _submitJournalService = submitJournalService;
        _approveJournalService = approveJournalService;
        _postJournalService = postJournalService;
    }

    public async Task ExecuteAsync(
        Guid billId,
        Guid tenantId,
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID is required.");

        if (tenantId == Guid.Empty)
            throw new ArgumentException("Tenant ID is required.");

        if (companyId == Guid.Empty)
            throw new ArgumentException("Company ID is required.");

        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.");

        await _unitOfWork.ExecuteInTransactionAsync(
            async ct =>
            {
                await ExecuteWithinTransactionAsync(
                    billId,
                    tenantId,
                    companyId,
                    userId,
                    ct);

                return true;
            },
            cancellationToken);
    }

    private async Task ExecuteWithinTransactionAsync(
        Guid billId,
        Guid tenantId,
        Guid companyId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Lock the bill to prevent concurrent posting.
        var bill = await _billRepository.GetForUpdateAsync(
            billId,
            tenantId,
            companyId,
            cancellationToken);

        if (bill is null)
            throw new KeyNotFoundException("Supplier bill not found.");

        if (bill.Status != BillStatus.Approved)
            throw new InvalidOperationException(
                "Only approved supplier bills can be posted.");

        if (bill.TotalAmount <= 0)
            throw new InvalidOperationException(
                "Supplier bill total must be greater than zero.");

        // Load AP control account configuration.
        var settings = await _settingsRepository.GetAsync(
            tenantId,
            companyId,
            bill.AccountingBookId,
            cancellationToken);

        if (settings is null)
            throw new InvalidOperationException(
                "Accounts Payable settings have not been configured for this accounting book.");

        var payableAccount = await GetAccountAsync(
            settings.PayableControlAccountId,
            tenantId,
            cancellationToken);

        var inputVatAccount = settings.InputVatAccountId.HasValue
            ? await GetAccountAsync(
                settings.InputVatAccountId.Value,
                tenantId,
                cancellationToken)
            : null;

        if (bill.TaxTotal > 0 && inputVatAccount is null)
            throw new InvalidOperationException(
                "An Input VAT account must be configured before posting a bill containing tax.");

        // Create the purchase journal.
        var journal = new JournalEntry(
            tenantId,
            companyId,
            bill.AccountingBookId,
            $"AP-{Guid.NewGuid():N}",
            $"Supplier bill {bill.BillNumber} - {bill.SupplierInvoiceNumber}",
            bill.InvoiceDate,
            JournalType.Purchase,
            JournalSource.AccountsPayable);

        // Debit each bill line's expense or asset account.
        foreach (var line in bill.Lines)
        {
            if (line.Subtotal <= 0)
                continue;

            var account = await GetAccountAsync(
                line.AccountId,
                tenantId,
                cancellationToken);

            journal.AddLine(
                line.AccountId,
                line.Description,
                new Money(line.Subtotal, bill.CurrencyCode),
                Money.Zero(bill.CurrencyCode),
                MapCashFlowActivity(account.CashFlowCategory));
        }

        // Debit Input VAT.
        if (bill.TaxTotal > 0)
        {
            journal.AddLine(
                inputVatAccount!.Id,
                "Input VAT",
                new Money(bill.TaxTotal, bill.CurrencyCode),
                Money.Zero(bill.CurrencyCode),
                MapCashFlowActivity(inputVatAccount.CashFlowCategory));
        }

        // Credit Accounts Payable.
        journal.AddLine(
            payableAccount.Id,
            $"Accounts payable - {bill.BillNumber}",
            Money.Zero(bill.CurrencyCode),
            new Money(bill.TotalAmount, bill.CurrencyCode),
            MapCashFlowActivity(payableAccount.CashFlowCategory));

        if (!journal.IsBalanced())
            throw new InvalidOperationException(
                "The supplier bill journal is not balanced.");

        // Execute the complete GL workflow within the existing transaction.
        await _createJournalEntryService.ExecuteWithinTransactionAsync(
            journal,
            tenantId,
            userId,
            cancellationToken);

        await _submitJournalService.ExecuteWithinTransactionAsync(
            journal.Id,
            tenantId,
            userId,
            cancellationToken);

        await _approveJournalService.ExecuteWithinTransactionAsync(
            journal.Id,
            tenantId,
            userId,
            cancellationToken);

        await _postJournalService.ExecuteWithinTransactionAsync(
            journal.Id,
            tenantId,
            userId,
            cancellationToken);

        // Mark the AP bill as posted in the same transaction.
        bill.MarkAsPosted();

        await _billRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Account> GetAccountAsync(
        Guid accountId,
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(
            accountId,
            tenantId,
            cancellationToken);

        if (account is null)
            throw new InvalidOperationException(
                $"Account {accountId} could not be found.");

        return account;
    }

    private static CashFlowActivity MapCashFlowActivity(
        CashFlowCategory category)
    {
        return category switch
        {
            CashFlowCategory.Operating => CashFlowActivity.Operating,
            CashFlowCategory.Investing => CashFlowActivity.Investing,
            CashFlowCategory.Financing => CashFlowActivity.Financing,
            _ => CashFlowActivity.Excluded
        };
    }
}
