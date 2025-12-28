using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Brokerage;

public class UpdateBrokerageInput
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase letters, numbers, and hyphens only")]
    public string Slug { get; set; } = null!;
    
    [MaxLength(255)]
    [RegularExpression(@"^[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid domain format")]
    public string? AuthorisedDomain { get; set; }
    
    public bool IsMasterAccount { get; set; }
    
    public List<CreateBrokerageLogoInput>? Logos { get; set; }
}