using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Company;

public class CompanyQuery : PaginationParams
{
    public string? Name { get; set; }
    public string? TradingName { get; set; }
    public string? Type { get; set; }
    public string? Abn { get; set; }
    public string? Acn { get; set; }
    public string? Email { get; set; }
    public string? Industry { get; set; }
    public Guid? BrokerId { get; set; }
}