namespace Zamm.Application.Payloads.InputModels.Asset;

public class AssetPersonResult
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = null!;
    public decimal Percent { get; set; }
}