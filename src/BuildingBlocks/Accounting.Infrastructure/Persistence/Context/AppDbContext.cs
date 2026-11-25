
using Accounting.Domain.ChartOfAccounts.Entities;
using Accounting.Domain.Common.Entities;
using Accounting.Domain.Common.Interfaces;
using Accounting.Domain.GeneralLedger.Entities;
using Accounting.Domain.Organization.Entities;
using Accounting.Domain.Tenancy.Entities;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistence.Context;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<AccountingBook> AccountingBooks => Set<AccountingBook>();
    public DbSet<ChartOfAccounts> ChartsOfAccounts => Set<ChartOfAccounts>();
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(ConfigureTenantFilter),
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    private void ConfigureTenantFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : TenantEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(entity =>
                _tenantContext.TenantId.HasValue &&
                entity.TenantId == _tenantContext.TenantId.Value);
    }
}
