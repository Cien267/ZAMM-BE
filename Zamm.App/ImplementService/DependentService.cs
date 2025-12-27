using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Dependent;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Depentdent;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;

namespace Zamm.Application.ImplementService;

public class DependentService : IDependentService
{
    private readonly IBaseRepository<Dependent> _baseDependentRepository;
    private readonly IBaseRepository<Person> _basePersonRepository;

    public DependentService(
        IBaseRepository<Dependent> baseDependentRepository,
        IBaseRepository<Person> basePersonRepository)
    {
        _baseDependentRepository = baseDependentRepository;
        _basePersonRepository = basePersonRepository;
    }

    public async Task<List<DependentResult>> GetDependentsByPersonIdAsync(Guid personId)
    {
        var dependents = await _baseDependentRepository
            .BuildQueryable(null, d => d.PersonId == personId)
            .Select(DependentResult.FromDependent)
            .ToListAsync();

        return dependents;
    }

    public async Task<DependentResult> GetDependentByIdAsync(Guid id)
    {
        var dependent = await _baseDependentRepository
            .BuildQueryable(null, d => d.Id == id)
            .Select(DependentResult.FromDependent)
            .FirstOrDefaultAsync();

        if (dependent == null)
        {
            throw new ResponseErrorObject("Dependent not found", StatusCodes.Status404NotFound);
        }

        return dependent;
    }

    public async Task<DependentResult> CreateDependentAsync(CreateDependentInput request)
    {
        var personExists = await _basePersonRepository.GetByIdAsync(request.PersonId);
        if (personExists == null)
        {
            throw new ResponseErrorObject("Person not found", StatusCodes.Status400BadRequest);
        }

        var currentYear = DateTime.Now.Year;
        if (request.YearOfBirth < 1900 || request.YearOfBirth > currentYear)
        {
            throw new ResponseErrorObject($"Year of birth must be between 1900 and {currentYear}", StatusCodes.Status400BadRequest);
        }

        var dependent = new Dependent
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            YearOfBirth = request.YearOfBirth,
            Gender = request.Gender,
            Relationship = request.Relationship,
            IsStudent = request.IsStudent,
            Notes = request.Notes,
            PersonId = request.PersonId
        };

        await _baseDependentRepository.CreateAsync(dependent);

        return await GetDependentByIdAsync(dependent.Id);
    }

    public async Task<DependentResult> UpdateDependentAsync(UpdateDependentInput request)
    {
        var dependent = await _baseDependentRepository.GetByIdAsync(request.Id);
        if (dependent == null)
        {
            throw new ResponseErrorObject("Dependent not found", StatusCodes.Status404NotFound);
        }

        // Validate PersonId exists if changed
        if (dependent.PersonId != request.PersonId)
        {
            var personExists = await _basePersonRepository.GetByIdAsync(request.PersonId);
            if (personExists == null)
            {
                throw new ResponseErrorObject("Person not found", StatusCodes.Status400BadRequest);
            }
        }

        // Validate year of birth
        var currentYear = DateTime.Now.Year;
        if (request.YearOfBirth < 1900 || request.YearOfBirth > currentYear)
        {
            throw new ResponseErrorObject($"Year of birth must be between 1900 and {currentYear}", StatusCodes.Status400BadRequest);
        }

        dependent.FullName = request.FullName;
        dependent.YearOfBirth = request.YearOfBirth;
        dependent.Gender = request.Gender;
        dependent.Relationship = request.Relationship;
        dependent.IsStudent = request.IsStudent;
        dependent.Notes = request.Notes;
        dependent.PersonId = request.PersonId;

        await _baseDependentRepository.UpdateAsync(dependent);

        return await GetDependentByIdAsync(dependent.Id);
    }

    public async Task DeleteDependentAsync(Guid id)
    {
        var dependent = await _baseDependentRepository.GetByIdAsync(id);
        if (dependent == null)
        {
            throw new ResponseErrorObject("Dependent not found", StatusCodes.Status404NotFound);
        }

        await _baseDependentRepository.DeleteAsync(id);
    }
}