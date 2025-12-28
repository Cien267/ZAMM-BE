using System.Linq.Expressions;

namespace Zamm.Application.Payloads.ResultModels.Brokerage;

public class BrokerageResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;
    public string? AuthorisedDomain { get; set; }
    public bool IsMasterAccount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<BrokerageLogoResult>? Logos { get; set; }
    public int UsersCount { get; set; }
    public int InvitationsCount { get; set; }

    public static Expression<Func<Domain.Entities.Brokerage, BrokerageResult>> FromBrokerage =>
        b => new BrokerageResult
        {
            Id = b.Id,
            Name = b.Name,
            Slug = b.Slug,
            AuthorisedDomain = b.AuthorisedDomain,
            IsMasterAccount = b.IsMasterAccount,
            CreatedAt = b.CreatedAt,
            Logos = b.Logos.Select(l => new BrokerageLogoResult
            {
                Id = l.Id,
                Url = l.Url,
                BrokerageId = l.BrokerageId
            }).ToList(),
            UsersCount = b.Users.Count,
            InvitationsCount = b.Invitations.Count
        };
}