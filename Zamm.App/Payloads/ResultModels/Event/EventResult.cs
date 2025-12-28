using System.Linq.Expressions;

namespace Zamm.Application.Payloads.ResultModels.Event;

public class EventResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Type { get; set; } = null!;
    public DateTime Date { get; set; }
    public bool IsSystem { get; set; }
    
    public bool IsRepeating { get; set; }
    public int? RepeatNumber { get; set; }
    public string? RepeatUnit { get; set; }
    
    public bool IsDismissed { get; set; }
    public DateTime? RepeatingDateDismissed { get; set; }
    
    public string? ModifiedValuesJson { get; set; }
    public string? ModifiedValuesObject { get; set; }
    
    public Guid AddedByUserId { get; set; }
    public string AddedByUserName { get; set; } = null!;
    
    public Guid? LiabilityId { get; set; }
    public string? LiabilityName { get; set; }
    
    public Guid? PersonId { get; set; }
    public string? PersonName { get; set; }
    
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    
    public List<EventFileResult>? Files { get; set; }

    public static Expression<Func<Domain.Entities.Event, EventResult>> FromEvent =>
        e => new EventResult
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Type = e.Type,
            Date = e.Date,
            IsSystem = e.IsSystem,
            IsRepeating = e.IsRepeating,
            RepeatNumber = e.RepeatNumber,
            RepeatUnit = e.RepeatUnit,
            IsDismissed = e.IsDismissed,
            RepeatingDateDismissed = e.RepeatingDateDismissed,
            ModifiedValuesJson = e.ModifiedValuesJson,
            ModifiedValuesObject = e.ModifiedValuesObject,
            AddedByUserId = e.AddedByUserId,
            AddedByUserName = e.AddedByUser.FullName ?? e.AddedByUser.UserName,
            LiabilityId = e.LiabilityId,
            LiabilityName = e.Liability != null ? e.Liability.Name : null,
            PersonId = e.PersonId,
            PersonName = e.Person != null ? e.Person.FirstName + " " + e.Person.LastName : null,
            CompanyId = e.CompanyId,
            CompanyName = e.Company != null ? e.Company.Name : null,
            Files = e.Files.Select(f => new EventFileResult
            {
                Id = f.Id,
                Filename = f.Filename,
                Url = f.Url,
                EventId = f.EventId,
                CreatedAt = f.CreatedAt
            }).ToList()
        };
}