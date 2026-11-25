
using Accounting.Identity.Contracts;

namespace Accounting.Identity.Services;

public interface ILoginService
{
    Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);
}
