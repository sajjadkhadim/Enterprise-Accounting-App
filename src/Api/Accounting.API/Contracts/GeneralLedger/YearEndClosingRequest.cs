using System.ComponentModel.DataAnnotations;

namespace Accounting.API.Contracts.GeneralLedger;
public sealed record YearEndClosingRequest(
    Guid RetainedEarningsAccountId);