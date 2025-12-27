namespace Zamm.Application.Payloads.ResultModels.Asset;

public class AssetLiabilityResult
{
    public Guid Id { get; set; }
    public Guid LiabilityId { get; set; }
    public string LiabilityName { get; set; } = null!;
}