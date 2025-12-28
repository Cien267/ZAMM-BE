using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Lender;

public class LenderQuery : PaginationParams
{
    public string? Name { get; set; }
    public string? Slug { get; set; }  
}