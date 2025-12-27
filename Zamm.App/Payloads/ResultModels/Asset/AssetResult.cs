using System.Linq.Expressions;
using Zamm.Application.Payloads.InputModels.Asset;
using Zamm.Application.Payloads.ResultModels.Address;
using Zamm.Application.Payloads.ResultModels.Common;
using Zamm.Application.Payloads.ResultModels.Depentdent;

namespace Zamm.Application.Payloads.ResultModels.Asset;

public class AssetResult : DataResponseBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    
    public AddressResult? Address { get; set; }
    
    public bool AddressOffPlan { get; set; }
    public string? PropertyType { get; set; }
    public string? ZoningType { get; set; }
    
    public decimal? Value { get; set; }
    public bool ValueIsCertified { get; set; }
    public DateTime? ValuationDate { get; set; }
    
    public bool IsInvestment { get; set; }
    public decimal? RentalIncomeValue { get; set; }
    public string? RentalIncomeFrequency { get; set; }
    public bool RentalHasAgent { get; set; }
    public string? RentalAgentContact { get; set; }
    
    public bool IsUnencumbered { get; set; }
    
    public List<AssetPersonResult>? AssetPeople { get; set; }
    public List<AssetCompanyResult>? AssetCompanies { get; set; }
    public List<AssetLiabilityResult>? AssetLiabilities { get; set; }

    public static Expression<Func<Domain.Entities.Asset, AssetResult>> FromAsset =>
        a => new AssetResult
        {
            Id = a.Id,
            Name = a.Name,
            Address = a.Address != null ? new AddressResult
            {
                Id = a.Address.Id,
                Level = a.Address.Level,
                Building = a.Address.Building,
                UnitNumber = a.Address.UnitNumber,
                StreetNumber = a.Address.StreetNumber,
                StreetName = a.Address.StreetName,
                Suburb = a.Address.Suburb,
                State = a.Address.State,
                Country = a.Address.Country,
                Postcode = a.Address.Postcode,
                OffPlan = a.Address.OffPlan,
            } : null,


            AddressOffPlan = a.AddressOffPlan,
            PropertyType = a.PropertyType,
            ZoningType = a.ZoningType,
            Value = a.Value,
            ValueIsCertified = a.ValueIsCertified,
            ValuationDate = a.ValuationDate,
            IsInvestment = a.IsInvestment,
            RentalIncomeValue = a.RentalIncomeValue,
            RentalIncomeFrequency = a.RentalIncomeFrequency,
            RentalHasAgent = a.RentalHasAgent,
            RentalAgentContact = a.RentalAgentContact,
            IsUnencumbered = a.IsUnencumbered,
            AssetPeople = a.AssetPeople.Select(ap => new AssetPersonResult
            {
                Id = ap.Id,
                PersonId = ap.PersonId,
                PersonName = ap.Person.FirstName + " " + ap.Person.LastName,
                Percent = ap.Percent
            }).ToList(),
            AssetCompanies = a.AssetCompanies.Select(ac => new AssetCompanyResult
            {
                Id = ac.Id,
                CompanyId = ac.CompanyId,
                CompanyName = ac.Company.Name,
                Percent = ac.Percent
            }).ToList(),
            AssetLiabilities = a.LiabilityAssets.Select(la => new AssetLiabilityResult
            {
                Id = la.Id,
                LiabilityId = la.LiabilityId,
                LiabilityName = la.Liability.Name ?? ""
            }).ToList()
        };
}