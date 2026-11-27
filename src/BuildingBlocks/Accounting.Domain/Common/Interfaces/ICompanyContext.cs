namespace Accounting.Domain.Common.Interfaces;

public interface ICompanyContext
{
    Guid? CompanyId { get; }

    bool HasCompany { get; }
}