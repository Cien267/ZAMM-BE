using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Brokerage;

public class BrokerageQuery : PaginationParams
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public bool? IsMasterAccount { get; set; }
}