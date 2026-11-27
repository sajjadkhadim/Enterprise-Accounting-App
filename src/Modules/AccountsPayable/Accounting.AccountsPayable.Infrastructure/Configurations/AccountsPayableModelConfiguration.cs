using Accounting.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Accounting.AccountsPayable.Infrastructure.Configurations;

public sealed class AccountsPayableModelConfiguration
    : IAppDbContextModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AccountsPayableModelConfiguration).Assembly);
    }
}