
namespace Accounting.Identity.Seeding;

public interface IDevelopmentSeedService
{
    Task SeedAsync(
        CancellationToken cancellationToken = default);
}
