using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Loan;

public class CreateLoanInput
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    
    [Required]
    public Guid LenderId { get; set; }
    
    public List<CreateInterestRateInput>? InterestRates { get; set; }
}