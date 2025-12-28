using System.Linq.Expressions;

namespace Zamm.Application.Payloads.ResultModels.Invitation;

public class InvitationResult
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public bool IsComplete { get; set; }
    public bool IsAdmin { get; set; }
    public Guid BrokerageId { get; set; }
    public string BrokerageName { get; set; } = null!;
    public Guid InviterId { get; set; }
    public string InviterName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public static Expression<Func<Domain.Entities.Invitation, InvitationResult>> FromInvitation =>
        i => new InvitationResult
        {
            Id = i.Id,
            Email = i.Email,
            IsComplete = i.IsComplete,
            IsAdmin = i.IsAdmin,
            BrokerageId = i.BrokerageId,
            BrokerageName = i.Brokerage.Name,
            InviterId = i.InviterId,
            InviterName = i.Inviter.FullName ?? i.Inviter.UserName,
            CreatedAt = i.CreatedAt
        };
}