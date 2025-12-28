using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Brokerage;

public class CreateBrokerageLogoInput
{
    [Required]
    [MaxLength(1000)]
    [Url]
    public string Url { get; set; } = null!;
}