using System.Linq.Expressions;
using Zamm.Application.Payloads.ResultModels.Address;
using Zamm.Application.Payloads.ResultModels.Common;
using Zamm.Application.Payloads.ResultModels.Depentdent;

namespace Zamm.Application.Payloads.ResultModels.Person;

public class PersonResult : DataResponseBase
{
    public Guid Id { get; set; }
    
    public string? Title { get; set; }
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public string? PreferredName { get; set; }
    public string FullName => $"{Title} {FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
    
    public DateTime? DateOfBirth { get; set; }
    public bool NotifyOfBirthday { get; set; }
    
    public string? Gender { get; set; }
    public string? MaritalStatus { get; set; }
    
    public string? Email { get; set; }
    public string? PhoneWork { get; set; }
    public string? PhoneMobile { get; set; }
    public string? PhonePreference { get; set; }
    
    public bool ActingOnTrust { get; set; }
    public string? TrustName { get; set; }
    
    public Guid? SpouseId { get; set; }
    public string? SpouseName { get; set; }
    
    public Guid BrokerId { get; set; }
    public string? BrokerName { get; set; }
    
    public AddressResult? Address { get; set; }
    public string? AddressLine { get; set; }
    public List<DependentResult>? Dependents { get; set; }
    /*public List<CompanyPersonResult>? Companies { get; set; }
    public List<AssetPersonResult>? Assets { get; set; }
    public List<LiabilityPersonResult>? Liabilities { get; set; }*/
    
    public static Expression<Func<Domain.Entities.Person, PersonResult>> FromPerson =>
        p => new PersonResult
        {
            Id = p.Id,
            Title = p.Title,
            FirstName = p.FirstName,
            MiddleName = p.MiddleName,
            LastName = p.LastName,
            PreferredName = p.PreferredName,
            DateOfBirth = p.DateOfBirth,
            NotifyOfBirthday = p.NotifyOfBirthday,
            Gender = p.Gender,
            MaritalStatus = p.MaritalStatus,
            Email = p.Email,
            PhoneWork = p.PhoneWork,
            PhoneMobile = p.PhoneMobile,
            PhonePreference = p.PhonePreference,
            ActingOnTrust = p.ActingOnTrust,
            TrustName = p.TrustName,
            SpouseId = p.SpouseId,
            SpouseName = p.Spouse != null ? p.Spouse.FirstName + " " + p.Spouse.LastName : null,
            BrokerId = p.BrokerId,
            BrokerName = p.Broker.FullName,
            Address = p.Address != null ? new AddressResult
            {
                Id = p.Address.Id,
                Level = p.Address.Level,
                Building = p.Address.Building,
                UnitNumber = p.Address.UnitNumber,
                StreetNumber = p.Address.StreetNumber,
                StreetName = p.Address.StreetName,
                Suburb = p.Address.Suburb,
                State = p.Address.State,
                Country = p.Address.Country,
                Postcode = p.Address.Postcode,
                OffPlan = p.Address.OffPlan,
            } : null,
            Dependents = p.Dependents.Select(d => new DependentResult
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
            }).ToList()
        };
}