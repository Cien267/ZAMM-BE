using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Company;

public class CreateCompanyPersonInput
{
    [Required]
    public Guid PersonId { get; set; }
}