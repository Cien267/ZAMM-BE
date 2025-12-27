using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Asset;

public class AssetQuery : PaginationParams
{
    public string? Name { get; set; }
    public bool? IsInvestment { get; set; }
    public string? ZoningType { get; set; }
    public string? PropertyType { get; set; }
}