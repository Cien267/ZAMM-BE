using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Event;

public class CreateEventFileInput
{
    [Required]
    [MaxLength(500)]
    public string Filename { get; set; } = null!;
    
    [Required]
    [MaxLength(1000)]
    [Url]
    public string Url { get; set; } = null!;
}