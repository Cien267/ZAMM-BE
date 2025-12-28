using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Loan;

public class CreateInterestRateInput
{
    [Required]
    [MaxLength(50)]
    public string RateType { get; set; } = null!;
    
    [Required]
    [Range(0, 100)]
    public decimal Rate { get; set; }
}