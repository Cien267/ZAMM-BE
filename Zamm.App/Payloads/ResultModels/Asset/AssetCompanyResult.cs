namespace Zamm.Application.Payloads.InputModels.Asset;

public class AssetCompanyResult
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public decimal Percent { get; set; }
}