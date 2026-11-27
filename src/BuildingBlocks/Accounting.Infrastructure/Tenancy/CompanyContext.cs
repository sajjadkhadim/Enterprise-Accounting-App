using Accounting.Domain.Common.Interfaces;

namespace Accounting.Infrastructure.Tenancy;

public sealed class CompanyContext : ICompanyContext
{
    public Guid? CompanyId { get; private set; }

    public bool HasCompany => CompanyId.HasValue;

    public void SetCompany(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException(
                "Company ID cannot be empty.",
                nameof(companyId));
        }

        if (CompanyId.HasValue && CompanyId.Value != companyId)
        {
            throw new InvalidOperationException(
                "Company context has already been assigned.");
        }

        CompanyId = companyId;
    }
}