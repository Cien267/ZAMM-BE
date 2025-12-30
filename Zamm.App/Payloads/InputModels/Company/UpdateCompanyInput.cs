using System.ComponentModel.DataAnnotations;
using Zamm.Application.Payloads.InputModels.Address;

namespace Zamm.Application.Payloads.InputModels.Company;

public class UpdateCompanyInput
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    
    [MaxLength(200)]
    public string? TradingName { get; set; }
    
    [MaxLength(100)]
    public string? Type { get; set; }
    
    [MaxLength(20)]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "ABN must be 11 digits")]
    public string? Abn { get; set; }
    
    [MaxLength(20)]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "ACN must be 9 digits")]
    public string? Acn { get; set; }
    
    public DateTime? RegistrationDate { get; set; }
    
    [MaxLength(20)]
    public string? PhoneWork { get; set; }
    
    [MaxLength(500)]
    [Url]
    public string? Website { get; set; }
    
    [EmailAddress]
    [MaxLength(256)]
    public string? Email { get; set; }
    
    [MaxLength(100)]
    public string? Industry { get; set; }
    
    public bool ActingOnTrust { get; set; }
    
    [MaxLength(200)]
    public string? TrustName { get; set; }
    
    [MaxLength(200)]
    public string? ExternalContactName { get; set; }
    
    [EmailAddress]
    [MaxLength(256)]
    public string? ExternalContactEmail { get; set; }
    
    [MaxLength(20)]
    public string? ExternalContactPhone { get; set; }
    
    public bool IsContactExistingPerson { get; set; }
    
    [Required]
    public Guid BrokerId { get; set; }
    
    public CreateAddressInput? Address { get; set; }
    
    public List<CreateCompanyPersonInput>? CompanyPeople { get; set; }
}