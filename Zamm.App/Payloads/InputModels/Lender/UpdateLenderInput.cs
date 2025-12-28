using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Lender;

public class UpdateLenderInput
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be lowercase letters, numbers, and hyphens only")]
    public string Slug { get; set; } = null!;
}