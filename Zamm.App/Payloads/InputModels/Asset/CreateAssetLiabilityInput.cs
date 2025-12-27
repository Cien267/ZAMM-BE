using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Asset;

public class CreateAssetLiabilityInput
{
    [Required]
    public Guid LiabilityId { get; set; }
}