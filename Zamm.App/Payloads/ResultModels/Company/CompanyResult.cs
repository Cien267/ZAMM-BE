using System.Linq.Expressions;
using Zamm.Application.Payloads.ResultModels.Address;

namespace Zamm.Application.Payloads.ResultModels.Company;

public class CompanyResult
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? TradingName { get; set; }
    public string? Type { get; set; }
    
    public string? Abn { get; set; }
    public string? Acn { get; set; }
    public DateTime? RegistrationDate { get; set; }
    
    public string? PhoneWork { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? Industry { get; set; }
    
    public bool ActingOnTrust { get; set; }
    public string? TrustName { get; set; }
    
    public string? ExternalContactName { get; set; }
    public string? ExternalContactEmail { get; set; }
    public string? ExternalContactPhone { get; set; }
    
    public bool IsContactExistingPerson { get; set; }
    
    public Guid BrokerId { get; set; }
    public string? BrokerName { get; set; }
    
    public AddressResult? Address { get; set; }
    
    public List<CompanyPersonResult>? CompanyPeople { get; set; }

    public static Expression<Func<Domain.Entities.Company, CompanyResult>> FromCompany =>
        c => new CompanyResult
        {
            Id = c.Id,
            Name = c.Name,
            TradingName = c.TradingName,
            Type = c.Type,
            Abn = c.Abn,
            Acn = c.Acn,
            RegistrationDate = c.RegistrationDate,
            PhoneWork = c.PhoneWork,
            Website = c.Website,
            Email = c.Email,
            Industry = c.Industry,
            ActingOnTrust = c.ActingOnTrust,
            TrustName = c.TrustName,
            ExternalContactName = c.ExternalContactName,
            ExternalContactEmail = c.ExternalContactEmail,
            ExternalContactPhone = c.ExternalContactPhone,
            IsContactExistingPerson = c.IsContactExistingPerson,
            BrokerId = c.BrokerId,
            BrokerName = c.Broker != null ? c.Broker.FullName ?? c.Broker.UserName : null,
            Address = c.Address != null ? new AddressResult
            {
                Id = c.Address.Id,
                Level = c.Address.Level,
                Building = c.Address.Building,
                UnitNumber = c.Address.UnitNumber,
                StreetNumber = c.Address.StreetNumber,
                StreetName = c.Address.StreetName,
                Suburb = c.Address.Suburb,
                State = c.Address.State,
                Country = c.Address.Country,
                Postcode = c.Address.Postcode,
                OffPlan = c.Address.OffPlan,
            } : null,
            CompanyPeople = c.CompanyPeople.Select(cp => new CompanyPersonResult
            {
                Id = cp.Id,
                PersonId = cp.PersonId,
                PersonName = cp.Person.FirstName + " " + cp.Person.LastName,
            }).ToList()
        };
}