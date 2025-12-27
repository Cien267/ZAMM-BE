using System.ComponentModel.DataAnnotations;
using Zamm.Application.Payloads.InputModels.Address;
using Zamm.Application.Payloads.InputModels.Dependent;

namespace Zamm.Application.Payloads.InputModels.Person;

public class CreatePersonInput
{
    public string? Title { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;
    
    [MaxLength(100)]
    public string? MiddleName { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;
    
    [MaxLength(100)]
    public string? PreferredName { get; set; }
    
    public DateTime? DateOfBirth { get; set; }
    public bool NotifyOfBirthday { get; set; }
    
    [MaxLength(50)]
    public string? Gender { get; set; }
    
    [MaxLength(50)]
    public string? MaritalStatus { get; set; }
    
    [EmailAddress]
    [MaxLength(256)]
    public string? Email { get; set; }
    
    [MaxLength(20)]
    public string? PhoneWork { get; set; }
    
    [MaxLength(20)]
    public string? PhoneMobile { get; set; }
    
    [MaxLength(20)]
    public string? PhonePreference { get; set; }

    public bool ActingOnTrust { get; set; } = false;
    
    [MaxLength(200)]
    public string? TrustName { get; set; }
    
    public Guid? SpouseId { get; set; }
    
    [Required]
    public Guid BrokerId { get; set; }
    
    public CreateAddressInput? Address { get; set; }
    public List<CreateDependentInput>? Dependents { get; set; }
}