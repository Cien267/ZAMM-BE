using System.Linq.Expressions;

namespace Zamm.Application.Payloads.ResultModels.Loan;

public class LoanResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid LenderId { get; set; }
    public string LenderName { get; set; } = null!;
    public int InterestRatesCount { get; set; }
    public List<InterestRateResult>? InterestRates { get; set; }
    public int LiabilitiesCount { get; set; }
    public static Expression<Func<Domain.Entities.Loan, LoanResult>> FromLoan =>
        loan => new LoanResult
        {
            Id = loan.Id,
            Name = loan.Name,
            LenderId = loan.LenderId,
            LenderName = loan.Lender.Name,
            InterestRatesCount = loan.InterestRates.Count,
            InterestRates = loan.InterestRates.Select(ir => new InterestRateResult
            {
                Id = ir.Id,
                RateType = ir.RateType,
                Rate = ir.Rate,
                LoanId = ir.LoanId
            }).ToList(),
            LiabilitiesCount = loan.Liabilities.Count
        };
}