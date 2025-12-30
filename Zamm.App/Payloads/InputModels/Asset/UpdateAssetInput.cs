using System.ComponentModel.DataAnnotations;
using Zamm.Application.Payloads.InputModels.Address;
using Zamm.Application.Payloads.InputModels.Dependent;

namespace Zamm.Application.Payloads.InputModels.Asset;

public class UpdateAssetInput
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    
    public CreateAddressInput? Address { get; set; }
    
    public bool AddressOffPlan { get; set; }
    
    [MaxLength(100)]
    public string? PropertyType { get; set; }
    
    [MaxLength(100)]
    public string? ZoningType { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? Value { get; set; }
    
    public bool ValueIsCertified { get; set; }
    
    public DateTime? ValuationDate { get; set; }
    
    public bool IsInvestment { get; set; }
    
    [Range(0, double.MaxValue)]
    public decimal? RentalIncomeValue { get; set; }
    
    [MaxLength(50)]
    public string? RentalIncomeFrequency { get; set; }
    
    public bool RentalHasAgent { get; set; }
    
    [MaxLength(200)]
    public string? RentalAgentContact { get; set; }
    
    public bool IsUnencumbered { get; set; }
    
    public List<CreateAssetPersonInput>? AssetPeople { get; set; }
    public List<CreateAssetCompanyInput>? AssetCompanies { get; set; }
    public List<CreateAssetLiabilityInput>? AssetLiabilities { get; set; }
}