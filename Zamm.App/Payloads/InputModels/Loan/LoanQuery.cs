using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Loan;

public class LoanQuery : PaginationParams
{
    public string? Name { get; set; }
    public Guid? LenderId { get; set; }
}