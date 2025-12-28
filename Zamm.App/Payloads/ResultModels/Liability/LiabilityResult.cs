using System.Linq.Expressions;

namespace Zamm.Application.Payloads.ResultModels.Liability;

public class LiabilityResult
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    
    public int? LoanTerm { get; set; }
    public int? InterestOnlyTerm { get; set; }
    public DateTime? StartDate { get; set; }
    
    public string? FinancePurpose { get; set; }
    
    public decimal? Amount { get; set; }
    public decimal? InitialBalance { get; set; }
    
    public int? IntroRateYears { get; set; }
    public decimal? IntroRatePercent { get; set; }
    
    public decimal? RepaymentAmount { get; set; }
    public string? RepaymentFrequency { get; set; }
    
    public decimal? DiscountPercent { get; set; }
    public decimal? SettlementRate { get; set; }
    
    public string? BankAccountName { get; set; }
    public string? BankAccountBsb { get; set; }
    public string? BankAccountNumber { get; set; }
    
    public string? OffsetAccountBsb { get; set; }
    public string? OffsetAccountNumber { get; set; }
    
    public Guid LoanId { get; set; }
    public string? LoanName { get; set; }
    public string? LenderName { get; set; }
    
    public List<LiabilityPersonResult>? LiabilityPeople { get; set; }
    public List<LiabilityCompanyResult>? LiabilityCompanies { get; set; }
    public List<LiabilityAssetResult>? LiabilityAssets { get; set; }
    public List<FixedRatePeriodResult>? FixedRatePeriods { get; set; }

    public static Expression<Func<Domain.Entities.Liability, LiabilityResult>> FromLiability =>
        l => new LiabilityResult
        {
            Id = l.Id,
            Name = l.Name,
            LoanTerm = l.LoanTerm,
            InterestOnlyTerm = l.InterestOnlyTerm,
            StartDate = l.StartDate,
            FinancePurpose = l.FinancePurpose,
            Amount = l.Amount,
            InitialBalance = l.InitialBalance,
            IntroRateYears = l.IntroRateYears,
            IntroRatePercent = l.IntroRatePercent,
            RepaymentAmount = l.RepaymentAmount,
            RepaymentFrequency = l.RepaymentFrequency,
            DiscountPercent = l.DiscountPercent,
            SettlementRate = l.SettlementRate,
            BankAccountName = l.BankAccountName,
            BankAccountBsb = l.BankAccountBsb,
            BankAccountNumber = l.BankAccountNumber,
            OffsetAccountBsb = l.OffsetAccountBsb,
            OffsetAccountNumber = l.OffsetAccountNumber,
            LoanId = l.LoanId,
            LoanName = l.Loan.Name,
            LenderName = l.Loan.Lender.Name,
            LiabilityPeople = l.LiabilityPeople.Select(lp => new LiabilityPersonResult
            {
                Id = lp.Id,
                PersonId = lp.PersonId,
                PersonName = lp.Person.FirstName + " " + lp.Person.LastName,
                Percent = lp.Percent
            }).ToList(),
            LiabilityCompanies = l.LiabilityCompanies.Select(lc => new LiabilityCompanyResult
            {
                Id = lc.Id,
                CompanyId = lc.CompanyId,
                CompanyName = lc.Company.Name,
                Percent = lc.Percent
            }).ToList(),
            LiabilityAssets = l.LiabilityAssets.Select(la => new LiabilityAssetResult
            {
                Id = la.Id,
                AssetId = la.AssetId,
                AssetName = la.Asset.Name
            }).ToList(),
            FixedRatePeriods = l.FixedRatePeriods.Select(frp => new FixedRatePeriodResult
            {
                Id = frp.Id,
                StartDate = frp.StartDate,
                Term = frp.Term,
                CustomRate = frp.CustomRate
            }).ToList()
        };
}