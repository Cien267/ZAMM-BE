namespace Zamm.Application.Payloads.ResultModels.Loan;

public class InterestRateResult
{
    public Guid Id { get; set; }
    public string RateType { get; set; } = null!;
    public decimal Rate { get; set; }
    public Guid LoanId { get; set; }
}