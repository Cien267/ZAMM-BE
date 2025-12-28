using System.Linq.Expressions;

namespace Zamm.Application.Payloads.ResultModels.Note;

public class NoteResult
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    
    public Guid? EditedById { get; set; }
    public string? EditedByName { get; set; }
    
    public Guid? LiabilityId { get; set; }
    public string? LiabilityName { get; set; }
    
    public Guid? EventId { get; set; }
    public string? EventTitle { get; set; }
    
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static Expression<Func<Domain.Entities.Note, NoteResult>> FromNote =>
        n => new NoteResult
        {
            Id = n.Id,
            Text = n.Text,
            AuthorId = n.AuthorId,
            AuthorName = n.Author.FullName ?? n.Author.UserName,
            EditedById = n.EditedById,
            EditedByName = n.EditedBy != null ? n.EditedBy.FullName ?? n.EditedBy.UserName : null,
            LiabilityId = n.LiabilityId,
            LiabilityName = n.Liability != null ? n.Liability.Name : null,
            EventId = n.EventId,
            EventTitle = n.Event != null ? n.Event.Title : null,
            PersonId = n.PersonId,
            PersonName = n.Person != null ? n.Person.FirstName + " " + n.Person.LastName : null,
            CompanyId = n.CompanyId,
            CompanyName = n.Company != null ? n.Company.Name : null,
            CreatedAt = n.CreatedAt,
            UpdatedAt = n.UpdatedAt
        };
}