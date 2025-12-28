using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Note;

public class CreateNoteInput
{
    [Required]
    public string Text { get; set; } = null!;
    
    [Required]
    public Guid AuthorId { get; set; }
    
    public Guid? LiabilityId { get; set; }
    
    public Guid? EventId { get; set; }
    
    public Guid? PersonId { get; set; }
    
    public Guid? CompanyId { get; set; }
}