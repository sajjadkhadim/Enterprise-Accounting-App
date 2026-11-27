using Accounting.AccountsPayable.Application.Bills.Services;
using Accounting.AccountsPayable.Application.Configuration.Services;
using Accounting.AccountsPayable.Application.PaymentTerms.Services;
using Accounting.AccountsPayable.Application.Suppliers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Accounting.AccountsPayable.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAccountsPayableApplication(
        this IServiceCollection services)
    {

        services.AddScoped<CreateSupplierService>();
        services.AddScoped<GetSupplierService>();
        services.AddScoped<UpdateSupplierService>();
        services.AddScoped<UpdateSupplierFinancialDetailsService>();
        services.AddScoped<ChangeSupplierStatusService>();
        services.AddScoped<SearchSuppliersService>();

        services.AddScoped<CreatePaymentTermService>();
        services.AddScoped<GetPaymentTermsService>();
        services.AddScoped<UpdatePaymentTermService>();
        services.AddScoped<ChangePaymentTermStatusService>();

        services.AddScoped<CreateSupplierBillService>();
        services.AddScoped<SubmitSupplierBillForApprovalService>();
        services.AddScoped<ApproveSupplierBillService>();
        services.AddScoped<RejectSupplierBillService>();

        services.AddScoped<ConfigureAccountsPayableService>();
        
        return services;
    }
}