using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Event;

public class UpdateEventInput
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = null!;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Type { get; set; } = null!;
    
    [Required]
    public DateTime Date { get; set; }
    
    public bool IsSystem { get; set; }
    
    public bool IsRepeating { get; set; }
    
    [Range(1, int.MaxValue)]
    public int? RepeatNumber { get; set; }
    
    [MaxLength(50)]
    public string? RepeatUnit { get; set; }
    
    public bool IsDismissed { get; set; }
    
    public DateTime? RepeatingDateDismissed { get; set; }
    
    public string? ModifiedValuesJson { get; set; }
    
    public string? ModifiedValuesObject { get; set; }
    
    [Required]
    public Guid AddedByUserId { get; set; }
    
    public Guid? LiabilityId { get; set; }
    
    public Guid? PersonId { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public List<CreateEventFileInput>? Files { get; set; }
}