using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Company;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Company;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Shared.Models;

namespace Zamm.Application.ImplementService;

public class CompanyService : ICompanyService
{
    private readonly IBaseRepository<Company> _baseCompanyRepository;
    private readonly IBaseRepository<User> _baseUserRepository;
    private readonly IBaseRepository<Person> _basePersonRepository;
    private readonly IBaseRepository<CompanyPerson> _baseCompanyPersonRepository;
    private readonly IAddressService _addressService;

    public CompanyService(
        IBaseRepository<Company> baseCompanyRepository,
        IBaseRepository<User> baseUserRepository,
        IBaseRepository<Person> basePersonRepository,
        IBaseRepository<CompanyPerson> baseCompanyPersonRepository,
        IAddressService addressService)
    {
        _baseCompanyRepository = baseCompanyRepository;
        _baseUserRepository = baseUserRepository;
        _basePersonRepository = basePersonRepository;
        _baseCompanyPersonRepository = baseCompanyPersonRepository;
        _addressService = addressService;
    }

    public async Task<PagedResult<CompanyResult>> GetListCompanyAsync(CompanyQuery companyQuery)
    {
        var query = _baseCompanyRepository.BuildQueryable(
            new List<string> { "Broker", "Address", "CompanyPeople.Person" },
            null
        );

        if (!string.IsNullOrEmpty(companyQuery.Name))
        {
            var name = companyQuery.Name.ToLower();
            query = query.Where(c => c.Name.ToLower().Contains(name));
        }

        if (!string.IsNullOrEmpty(companyQuery.TradingName))
        {
            var tradingName = companyQuery.TradingName.ToLower();
            query = query.Where(c => c.TradingName != null && c.TradingName.ToLower().Contains(tradingName));
        }

        if (!string.IsNullOrEmpty(companyQuery.Type))
        {
            var type = companyQuery.Type.ToLower();
            query = query.Where(c => c.Type != null && c.Type.ToLower().Contains(type));
        }

        if (!string.IsNullOrEmpty(companyQuery.Abn))
        {
            query = query.Where(c => c.Abn != null && c.Abn.Contains(companyQuery.Abn));
        }

        if (!string.IsNullOrEmpty(companyQuery.Acn))
        {
            query = query.Where(c => c.Acn != null && c.Acn.Contains(companyQuery.Acn));
        }

        if (!string.IsNullOrEmpty(companyQuery.Email))
        {
            var email = companyQuery.Email.ToLower();
            query = query.Where(c => c.Email != null && c.Email.ToLower().Contains(email));
        }

        if (!string.IsNullOrEmpty(companyQuery.Industry))
        {
            var industry = companyQuery.Industry.ToLower();
            query = query.Where(c => c.Industry != null && c.Industry.ToLower().Contains(industry));
        }

        if (companyQuery.BrokerId.HasValue)
        {
            query = query.Where(c => c.BrokerId == companyQuery.BrokerId.Value);
        }

        var totalCount = await query.CountAsync();

        query = _baseCompanyRepository.ApplySorting(query, companyQuery.SortBy, companyQuery.SortDescending);

        var results = await query
            .Skip((companyQuery.PageNumber - 1) * companyQuery.PageSize)
            .Take(companyQuery.PageSize)
            .Select(CompanyResult.FromCompany)
            .ToListAsync();

        return new PagedResult<CompanyResult>(results, totalCount, companyQuery.PageNumber, companyQuery.PageSize);
    }

    public async Task<CompanyResult> GetCompanyByIdAsync(Guid id)
    {
        var company = await _baseCompanyRepository
            .BuildQueryable(
                new List<string> { "Broker", "Address", "CompanyPeople.Person" },
                c => c.Id == id
            )
            .Select(CompanyResult.FromCompany)
            .FirstOrDefaultAsync();

        if (company == null)
        {
            throw new ResponseErrorObject("Company not found", StatusCodes.Status404NotFound);
        }

        return company;
    }

    public async Task<CompanyResult> CreateCompanyAsync(CreateCompanyInput request)
    {
        var brokerExists = await _baseUserRepository.GetByIdAsync(request.BrokerId);
        if (brokerExists == null)
        {
            throw new ResponseErrorObject("Broker not found", StatusCodes.Status400BadRequest);
        }

        if (!string.IsNullOrEmpty(request.Abn))
        {
            var abnExists = await _baseCompanyRepository.GetAsync(c => c.Abn == request.Abn);
            if (abnExists != null)
            {
                throw new ResponseErrorObject("ABN already exists", StatusCodes.Status400BadRequest);
            }
        }

        if (!string.IsNullOrEmpty(request.Acn))
        {
            var acnExists = await _baseCompanyRepository.GetAsync(c => c.Acn == request.Acn);
            if (acnExists != null)
            {
                throw new ResponseErrorObject("ACN already exists", StatusCodes.Status400BadRequest);
            }
        }

        if (request.CompanyPeople != null && request.CompanyPeople.Any())
        {
            foreach (var cp in request.CompanyPeople)
            {
                var personExists = await _basePersonRepository.GetByIdAsync(cp.PersonId);
                if (personExists == null)
                {
                    throw new ResponseErrorObject($"Person with ID {cp.PersonId} not found", StatusCodes.Status400BadRequest);
                }
            }
        }

        var addressId = await _addressService.UpsertAddressAsync(request.Address);

        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TradingName = request.TradingName,
            Type = request.Type,
            Abn = request.Abn,
            Acn = request.Acn,
            RegistrationDate = request.RegistrationDate,
            PhoneWork = request.PhoneWork,
            Website = request.Website,
            Email = request.Email,
            Industry = request.Industry,
            ActingOnTrust = request.ActingOnTrust,
            TrustName = request.TrustName,
            ExternalContactName = request.ExternalContactName,
            ExternalContactEmail = request.ExternalContactEmail,
            ExternalContactPhone = request.ExternalContactPhone,
            IsContactExistingPerson = request.IsContactExistingPerson,
            BrokerId = request.BrokerId,
            AddressId = addressId
        };

        await _baseCompanyRepository.CreateAsync(company);

        if (request.CompanyPeople != null && request.CompanyPeople.Any())
        {
            var companyPeople = request.CompanyPeople.Select(cp => new CompanyPerson
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                PersonId = cp.PersonId,
            }).ToList();

            await _baseCompanyPersonRepository.CreateAsync(companyPeople);
        }

        return await GetCompanyByIdAsync(company.Id);
    }

    public async Task<CompanyResult> UpdateCompanyAsync(Guid companyId, UpdateCompanyInput request)
    {
        var company = await _baseCompanyRepository.GetByIdAsync(companyId);
        if (company == null)
        {
            throw new ResponseErrorObject("Company not found", StatusCodes.Status404NotFound);
        }

        var brokerExists = await _baseUserRepository.GetByIdAsync(request.BrokerId);
        if (brokerExists == null)
        {
            throw new ResponseErrorObject("Broker not found", StatusCodes.Status400BadRequest);
        }

        if (!string.IsNullOrEmpty(request.Abn) && company.Abn != request.Abn)
        {
            var abnExists = await _baseCompanyRepository.GetAsync(c => c.Abn == request.Abn && c.Id != companyId);
            if (abnExists != null)
            {
                throw new ResponseErrorObject("ABN already exists", StatusCodes.Status400BadRequest);
            }
        }

        if (!string.IsNullOrEmpty(request.Acn) && company.Acn != request.Acn)
        {
            var acnExists = await _baseCompanyRepository.GetAsync(c => c.Acn == request.Acn && c.Id != companyId);
            if (acnExists != null)
            {
                throw new ResponseErrorObject("ACN already exists", StatusCodes.Status400BadRequest);
            }
        }

        if (request.CompanyPeople != null && request.CompanyPeople.Any())
        {
            foreach (var cp in request.CompanyPeople)
            {
                var personExists = await _basePersonRepository.GetByIdAsync(cp.PersonId);
                if (personExists == null)
                {
                    throw new ResponseErrorObject($"Person with ID {cp.PersonId} not found", StatusCodes.Status400BadRequest);
                }
            }
        }

        var addressId = await _addressService.UpsertAddressAsync(request.Address, company.AddressId);

        company.Name = request.Name;
        company.TradingName = request.TradingName;
        company.Type = request.Type;
        company.Abn = request.Abn;
        company.Acn = request.Acn;
        company.RegistrationDate = request.RegistrationDate;
        company.PhoneWork = request.PhoneWork;
        company.Website = request.Website;
        company.Email = request.Email;
        company.Industry = request.Industry;
        company.ActingOnTrust = request.ActingOnTrust;
        company.TrustName = request.TrustName;
        company.ExternalContactName = request.ExternalContactName;
        company.ExternalContactEmail = request.ExternalContactEmail;
        company.ExternalContactPhone = request.ExternalContactPhone;
        company.IsContactExistingPerson = request.IsContactExistingPerson;
        company.BrokerId = request.BrokerId;
        company.AddressId = addressId;

        await _baseCompanyRepository.UpdateAsync(company);

        if (request.CompanyPeople != null)
        {
            await _baseCompanyPersonRepository.DeleteAsync(cp => cp.CompanyId == companyId);

            if (request.CompanyPeople.Any())
            {
                var companyPeople = request.CompanyPeople.Select(cp => new CompanyPerson
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    PersonId = cp.PersonId,
                }).ToList();

                await _baseCompanyPersonRepository.CreateAsync(companyPeople);
            }
        }

        return await GetCompanyByIdAsync(company.Id);
    }

    public async Task DeleteCompanyAsync(Guid id)
    {
        var company = await _baseCompanyRepository.GetByIdAsync(id);
        if (company == null)
        {
            throw new ResponseErrorObject("Company not found", StatusCodes.Status404NotFound);
        }

        var hasAssets = await _baseCompanyRepository
            .BuildQueryable(new List<string> { "AssetCompanies" }, c => c.Id == id)
            .AnyAsync(c => c.AssetCompanies.Any());

        if (hasAssets)
        {
            throw new ResponseErrorObject("Cannot delete company with associated assets. Please remove asset associations first.", StatusCodes.Status400BadRequest);
        }

        var hasLiabilities = await _baseCompanyRepository
            .BuildQueryable(new List<string> { "LiabilityCompanies" }, c => c.Id == id)
            .AnyAsync(c => c.LiabilityCompanies.Any());

        if (hasLiabilities)
        {
            throw new ResponseErrorObject("Cannot delete company with associated liabilities. Please remove liability associations first.", StatusCodes.Status400BadRequest);
        }

        await _baseCompanyRepository.DeleteAsync(id);
    }
}