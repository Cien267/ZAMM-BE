using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Asset;

public class CreateAssetCompanyInput
{
    [Required]
    public Guid CompanyId { get; set; }
    
    [Required]
    [Range(0, 100)]
    public decimal Percent { get; set; }
}