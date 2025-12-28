using System.Linq.Expressions;
using Zamm.Application.Payloads.ResultModels.Loan;

namespace Zamm.Application.Payloads.ResultModels.Lender;

public class LenderResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public List<LoanResult>? Loans { get; set; }

    public static Expression<Func<Domain.Entities.Lender, LenderResult>> FromLender =>
        lender => new LenderResult
        {
            Id = lender.Id,
            Name = lender.Name,
            Slug = lender.Slug,
            Loans = lender.Loans.Select(loan => new LoanResult
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
            }).ToList()
        };
}