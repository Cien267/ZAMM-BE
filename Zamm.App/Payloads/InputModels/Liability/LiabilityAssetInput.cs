using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Liability;

public class LiabilityAssetInput
{
    [Required]
    public Guid AssetId { get; set; }
}