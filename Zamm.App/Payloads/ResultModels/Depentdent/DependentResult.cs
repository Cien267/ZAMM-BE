using System.Linq.Expressions;
using Zamm.Domain.Entities;

namespace Zamm.Application.Payloads.ResultModels.Depentdent;

public class DependentResult
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public int YearOfBirth { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? Relationship { get; set; }
    public bool? IsStudent { get; set; }
    public string? Notes { get; set; }
    public Guid PersonId { get; set; }

    public static Expression<Func<Dependent, DependentResult>> FromDependent =>
        d => new DependentResult
        {
            Id = d.Id,
            FullName = d.FullName,
            YearOfBirth = d.YearOfBirth,
            Age = DateTime.Now.Year - d.YearOfBirth,
            Gender = d.Gender,
            Relationship = d.Relationship,
            IsStudent = d.IsStudent,
            Notes = d.Notes,
            PersonId = d.PersonId
        };
}