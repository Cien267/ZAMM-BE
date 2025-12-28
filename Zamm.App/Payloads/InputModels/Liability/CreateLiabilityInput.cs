using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Liability;

public class CreateLiabilityInput
{
    [MaxLength(200)]
    public string? Name { get; set; }
    
    [Range(1, 360)]
    public int? LoanTerm { get; set; }
    
    [Range(0, 360)]
    public int? InterestOnlyTerm { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    [MaxLength(200)]
    public string? FinancePurpose { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? Amount { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? InitialBalance { get; set; }
    
    [Range(0, 30)]
    public int? IntroRateYears { get; set; }
    
    [Range(0, 100)]
    public decimal? IntroRatePercent { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? RepaymentAmount { get; set; }
    
    [MaxLength(50)]
    public string? RepaymentFrequency { get; set; }
    
    [Range(0, 100)]
    public decimal? DiscountPercent { get; set; }
    
    [Range(0, 100)]
    public decimal? SettlementRate { get; set; }
    
    [MaxLength(200)]
    public string? BankAccountName { get; set; }
    
    [MaxLength(10)]
    public string? BankAccountBsb { get; set; }
    
    [MaxLength(20)]
    public string? BankAccountNumber { get; set; }
    
    [MaxLength(10)]
    public string? OffsetAccountBsb { get; set; }
    
    [MaxLength(20)]
    public string? OffsetAccountNumber { get; set; }
    
    [Required]
    public Guid LoanId { get; set; }
    
    public List<LiabilityPersonInput>? LiabilityPeople { get; set; }
    public List<LiabilityCompanyInput>? LiabilityCompanies { get; set; }
    public List<LiabilityAssetInput>? LiabilityAssets { get; set; }
    public List<FixedRatePeriodInput>? FixedRatePeriods { get; set; }
}