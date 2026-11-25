
using Accounting.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Accounting.GeneralLedger.Infrastructure.Persistence;

public sealed class GeneralLedgerModelConfiguration
    : IAppDbContextModelConfiguration
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(GeneralLedgerModelConfiguration).Assembly);
    }
}
