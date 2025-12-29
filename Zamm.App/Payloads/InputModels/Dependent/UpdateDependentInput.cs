using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Dependent;

public class UpdateDependentInput
{
    
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = null!;
    
    [Required]
    [Range(1900, 2100)]
    public int YearOfBirth { get; set; }
    
    [MaxLength(50)]
    public string? Gender { get; set; }
    
    [MaxLength(100)]
    public string? Relationship { get; set; }
    
    public bool? IsStudent { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    [Required]
    public Guid PersonId { get; set; }
}