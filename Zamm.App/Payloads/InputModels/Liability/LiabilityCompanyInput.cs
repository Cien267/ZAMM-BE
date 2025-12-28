using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Liability;

public class LiabilityCompanyInput
{
    [Required]
    public Guid CompanyId { get; set; }
    
    [Required]
    [Range(0, 100)]
    public decimal Percent { get; set; }
}