using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Liability;

public class LiabilityQuery : PaginationParams
{
    public string? Name { get; set; }
    public Guid? LoanId { get; set; }
    public string? FinancePurpose { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }

}