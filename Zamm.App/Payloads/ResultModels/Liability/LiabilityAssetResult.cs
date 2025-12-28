namespace Zamm.Application.Payloads.ResultModels.Liability;

public class LiabilityAssetResult
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = null!;
}