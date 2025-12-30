using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Note;

public class UpdateNoteInput
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string Text { get; set; } = null!;
    
    [Required]
    public Guid EditedById { get; set; }
    
    public Guid? LiabilityId { get; set; }
    
    public Guid? EventId { get; set; }
    
    public Guid? PersonId { get; set; }
    
    public Guid? CompanyId { get; set; }
}