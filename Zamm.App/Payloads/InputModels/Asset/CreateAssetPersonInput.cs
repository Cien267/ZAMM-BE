using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Asset;

public class CreateAssetPersonInput
{
    [Required]
    public Guid PersonId { get; set; }
    
    [Required]
    [Range(0, 100)]
    public decimal Percent { get; set; }
}