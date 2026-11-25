using Microsoft.EntityFrameworkCore;

namespace Accounting.Infrastructure.Persistence.Configurations;

public interface IAppDbContextModelConfiguration
{
    void Configure(ModelBuilder modelBuilder);
}