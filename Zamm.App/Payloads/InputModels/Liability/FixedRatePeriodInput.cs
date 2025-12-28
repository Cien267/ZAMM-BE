using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Liability;

public class FixedRatePeriodInput
{
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    [Range(1, 30)]
    public int Term { get; set; }
    
    [Range(0, 100)]
    public decimal? CustomRate { get; set; }
}