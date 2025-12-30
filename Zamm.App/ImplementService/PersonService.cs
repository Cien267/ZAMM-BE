using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Address;
using Zamm.Application.Payloads.InputModels.Person;
using Zamm.Application.Payloads.Responses;
using Zamm.Shared.Models;
using Zamm.Application.Payloads.ResultModels.Person;

namespace Zamm.Application.ImplementService
{
    public class PersonService : IPersonService
    {
        private readonly IBaseRepository<Person> _basePersonRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<Dependent> _baseDependentRepository;
        private readonly IAddressService _addressService;
        public PersonService(
            IBaseRepository<Person> basePersonRepository, 
            IPersonRepository personRepository, 
            IBaseRepository<User> baseUserRepository,
            IBaseRepository<Dependent> baseDependentRepository,
            IHttpContextAccessor contextAccessor,
            IAddressService addressService)
        {
            _basePersonRepository = basePersonRepository;
            _personRepository = personRepository;
            _baseUserRepository = baseUserRepository;
            _baseDependentRepository = baseDependentRepository;
            _contextAccessor = contextAccessor;
            _addressService = addressService;
        }

        public async Task<PagedResult<PersonResult>> GetListPeopleAsync(PersonQuery personQuery)
        {
           
            var query = _basePersonRepository.BuildQueryable(
                new List<string> { "Spouse", "Broker", "Address", "Dependents" }, 
                null
            );
            
            if (!string.IsNullOrEmpty(personQuery.FirstName))
            {
                var firstName = personQuery.FirstName.ToLower();
                query = query.Where(p => p.FirstName.ToLower().Contains(firstName));
            }
            
            if (!string.IsNullOrEmpty(personQuery.LastName))
            {
                var lastName = personQuery.LastName.ToLower();
                query = query.Where(p => p.LastName.ToLower().Contains(lastName));
            }
            
            if (!string.IsNullOrEmpty(personQuery.Email))
            {
                var email = personQuery.Email.ToLower();
                query = query.Where(p => p.Email != null && p.Email.ToLower().Contains(email));
            }
            
            if (!string.IsNullOrEmpty(personQuery.Phone))
            {
                var phone = personQuery.Phone;
                query = query.Where(p => 
                    (p.PhoneMobile != null && p.PhoneMobile.Contains(phone)) ||
                    (p.PhoneWork != null && p.PhoneWork.Contains(phone)));
            }
            
            if (personQuery.BrokerId.HasValue)
            {
                query = query.Where(c => c.BrokerId == personQuery.BrokerId.Value);
            }
            
            var totalCount = await query.CountAsync();
            
            query = _basePersonRepository.ApplySorting(query, personQuery.SortBy, personQuery.SortDescending);
            
            var results = await query
                .Skip((personQuery.PageNumber - 1) * personQuery.PageSize)
                .Take(personQuery.PageSize)
                .Select(PersonResult.FromPerson)
                .ToListAsync();
            
            return new PagedResult<PersonResult>(results, totalCount, personQuery.PageNumber, personQuery.PageSize);
        }
        
        public async Task<PersonResult> GetPersonByIdAsync(Guid id)
        {
            var person = await _basePersonRepository
                .BuildQueryable(
                    new List<string> { "Spouse", "Broker", "Address", "Dependents" },
                    p => p.Id == id
                )
                .Select(PersonResult.FromPerson)
                .FirstOrDefaultAsync();

            if (person == null)
            {
                throw new ResponseErrorObject("Person not found", StatusCodes.Status404NotFound);
            }

            return person;
        }

        public async Task<PersonResult> CreatePersonAsync(CreatePersonInput request)
        {
            var brokerExists = await _baseUserRepository.GetByIdAsync(request.BrokerId);
            if (brokerExists == null)
            {
                throw new ResponseErrorObject("Broker not found", StatusCodes.Status400BadRequest);
            }

            if (request.SpouseId.HasValue)
            {
                var spouseExists = await _basePersonRepository.GetByIdAsync(request.SpouseId.Value);
                if (spouseExists == null)
                {
                    throw new ResponseErrorObject("Spouse not found", StatusCodes.Status400BadRequest);
                }
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailExists = await _basePersonRepository.GetAsync(p => p.Email == request.Email);
                if (emailExists != null)
                {
                    throw new ResponseErrorObject("Email already exists", StatusCodes.Status400BadRequest);
                }
            }
            
            if (request.Dependents != null && request.Dependents.Any())
            {
                var currentYear = DateTime.Now.Year;
                foreach (var dependent in request.Dependents)
                {
                    if (dependent.YearOfBirth < 1900 || dependent.YearOfBirth > currentYear)
                    {
                        throw new ResponseErrorObject($"Dependent '{dependent.FullName}' has invalid year of birth. Must be between 1900 and {currentYear}", StatusCodes.Status400BadRequest);
                    }
                }
            }
            
            var addressId = await _addressService.UpsertAddressAsync(request.Address);

            var person = new Person
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                PreferredName = request.PreferredName,
                DateOfBirth = request.DateOfBirth,
                NotifyOfBirthday = request.NotifyOfBirthday,
                Gender = request.Gender,
                MaritalStatus = request.MaritalStatus,
                Email = request.Email,
                PhoneWork = request.PhoneWork,
                PhoneMobile = request.PhoneMobile,
                PhonePreference = request.PhonePreference,
                ActingOnTrust = request.ActingOnTrust,
                TrustName = request.TrustName,
                SpouseId = request.SpouseId,
                BrokerId = request.BrokerId,
                AddressId = addressId
            };

            await _basePersonRepository.CreateAsync(person);
            
            if (request.Dependents != null && request.Dependents.Any())
            {
                var dependents = request.Dependents.Select(d => new Dependent
                {
                    Id = Guid.NewGuid(),
                    FullName = d.FullName,
                    YearOfBirth = d.YearOfBirth,
                    Gender = d.Gender,
                    Relationship = d.Relationship,
                    IsStudent = d.IsStudent,
                    Notes = d.Notes,
                    PersonId = person.Id
                }).ToList();

                await _baseDependentRepository.CreateAsync(dependents);
            }

            return await GetPersonByIdAsync(person.Id);
        }

        public async Task<PersonResult> UpdatePersonAsync(Guid personId, UpdatePersonInput request)
        {
            var person = await _basePersonRepository.GetByIdAsync(personId);
            if (person == null)
            {
                throw new ResponseErrorObject("Person not found", StatusCodes.Status404NotFound);
            }

            var brokerExists = await _baseUserRepository.GetByIdAsync(request.BrokerId);
            if (brokerExists == null)
            {
                throw new ResponseErrorObject( "Broker not found", StatusCodes.Status400BadRequest);
            }

            if (request.SpouseId.HasValue)
            {
                var spouseExists = await _basePersonRepository.GetByIdAsync(request.SpouseId.Value);
                if (spouseExists == null)
                {
                    throw new ResponseErrorObject( "Spouse not found", StatusCodes.Status400BadRequest);
                }

                if (request.SpouseId.Value == personId)
                {
                    throw new ResponseErrorObject("Person cannot be their own spouse", StatusCodes.Status400BadRequest);
                }
            }

            if (!string.IsNullOrEmpty(request.Email) && request.Email != person.Email)
            {
                var emailExists = await _basePersonRepository.GetAsync(p => p.Email == request.Email && p.Id != personId);
                if (emailExists != null)
                {
                    throw new ResponseErrorObject("Email already exists", StatusCodes.Status400BadRequest);
                }
            }
            
            if (request.Dependents != null && request.Dependents.Any())
            {
                var currentYear = DateTime.Now.Year;
                foreach (var dependent in request.Dependents)
                {
                    if (dependent.YearOfBirth < 1900 || dependent.YearOfBirth > currentYear)
                    {
                        throw new ResponseErrorObject($"Dependent '{dependent.FullName}' has invalid year of birth. Must be between 1900 and {currentYear}", StatusCodes.Status400BadRequest);
                    }
                }
            }
            
            var addressId = await _addressService.UpsertAddressAsync(request.Address, person.AddressId);

            person.Title = request.Title;
            person.FirstName = request.FirstName;
            person.MiddleName = request.MiddleName;
            person.LastName = request.LastName;
            person.PreferredName = request.PreferredName;
            person.DateOfBirth = request.DateOfBirth;
            person.NotifyOfBirthday = request.NotifyOfBirthday;
            person.Gender = request.Gender;
            person.MaritalStatus = request.MaritalStatus;
            person.Email = request.Email;
            person.PhoneWork = request.PhoneWork;
            person.PhoneMobile = request.PhoneMobile;
            person.PhonePreference = request.PhonePreference;
            person.ActingOnTrust = request.ActingOnTrust;
            person.TrustName = request.TrustName;
            person.SpouseId = request.SpouseId;
            person.BrokerId = request.BrokerId;
            person.AddressId = addressId;

            await _basePersonRepository.UpdateAsync(person);

            if (request.Dependents != null)
            {
                await _baseDependentRepository.DeleteAsync(d => d.PersonId == personId);

                if (request.Dependents.Any())
                {
                    var dependents = request.Dependents.Select(d => new Dependent
                    {
                        Id = Guid.NewGuid(),
                        FullName = d.FullName,
                        YearOfBirth = d.YearOfBirth,
                        Gender = d.Gender,
                        Relationship = d.Relationship,
                        IsStudent = d.IsStudent,
                        Notes = d.Notes,
                        PersonId = personId
                    }).ToList();

                    await _baseDependentRepository.CreateAsync(dependents);
                }
            }
            
            return await GetPersonByIdAsync(person.Id);
        }

        public async Task DeletePersonAsync(Guid id)
        {
            var person = await _basePersonRepository.GetByIdAsync(id);
            if (person == null)
            {
                throw new ResponseErrorObject("Person not found", StatusCodes.Status404NotFound);
            }

            var hasCompanies = await _basePersonRepository
                .BuildQueryable(new List<string> { "CompanyPeople" }, p => p.Id == id)
                .AnyAsync(p => p.CompanyPeople.Any());

            var hasAssets = await _basePersonRepository
                .BuildQueryable(new List<string> { "AssetPeople" }, p => p.Id == id)
                .AnyAsync(p => p.AssetPeople.Any());

            var hasLiabilities = await _basePersonRepository
                .BuildQueryable(new List<string> { "LiabilityPeople" }, p => p.Id == id)
                .AnyAsync(p => p.LiabilityPeople.Any());

            if (hasCompanies || hasAssets || hasLiabilities)
            {
                throw new ResponseErrorObject(
                    "Cannot delete person with associated companies, assets, or liabilities. Please remove associations first.", StatusCodes.Status400BadRequest);
            }

            var isSpouse = await _basePersonRepository.GetAsync(p => p.SpouseId == id);
            if (isSpouse != null)
            {
                throw new ResponseErrorObject(
                    "Cannot delete person who is marked as a spouse of another person. Please update the spouse relationship first.", StatusCodes.Status400BadRequest);
            }

            await _basePersonRepository.DeleteAsync(id);
        }
    }
}
