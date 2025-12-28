using System.ComponentModel.DataAnnotations;

namespace Zamm.Application.Payloads.InputModels.Invitation;

public class CreateInvitationInput
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = null!;
    
    public bool IsAdmin { get; set; }
    
    [Required]
    public Guid BrokerageId { get; set; }
    
    [Required]
    public Guid InviterId { get; set; }
}