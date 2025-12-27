using Zamm.Application.Payloads.ResultModels.Common;

namespace Zamm.Application.Payloads.ResultModels.Address;

public class AddressResult : DataResponseBase
{
    public Guid Id { get; set; }
    
    public string? Level { get; set; }
    
    public string? Building { get; set; }
    
    public string? UnitNumber { get; set; }
    
    public string? StreetNumber { get; set; }
    
    public string? StreetName { get; set; }
    
    public string? Suburb { get; set; }
    
    public string? State { get; set; }
    
    public string? Country { get; set; }
    
    public string? Postcode { get; set; }

    public bool OffPlan { get; set; } = false;
}