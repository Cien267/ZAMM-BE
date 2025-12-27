using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Address;

public class CreateAddressInput
{
    public Guid? Id { get; set; }
    
    [MaxLength(200)]
    public string? Level { get; set; }
    
    [MaxLength(200)]
    public string? Building { get; set; }
    
    [MaxLength(200)]
    public string? UnitNumber { get; set; }
    
    [MaxLength(200)]
    public string? StreetNumber { get; set; }
    
    [MaxLength(200)]
    public string? StreetName { get; set; }
    
    [MaxLength(200)]
    public string? Suburb { get; set; }
    
    [MaxLength(100)]
    public string? State { get; set; }
    
    [MaxLength(100)]
    public string? Country { get; set; }
    
    [MaxLength(20)]
    public string? Postcode { get; set; }

    public bool OffPlan { get; set; } = false;
}