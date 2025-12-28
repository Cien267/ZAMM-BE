using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Event;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Event;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Shared.Models;

namespace Zamm.Application.ImplementService;

public class EventService : IEventService
{
    private readonly IBaseRepository<Event> _baseEventRepository;
    private readonly IBaseRepository<EventFile> _baseEventFileRepository;
    private readonly IBaseRepository<User> _baseUserRepository;
    private readonly IBaseRepository<Liability> _baseLiabilityRepository;
    private readonly IBaseRepository<Person> _basePersonRepository;
    private readonly IBaseRepository<Company> _baseCompanyRepository;

    public EventService(
        IBaseRepository<Event> baseEventRepository,
        IBaseRepository<EventFile> baseEventFileRepository,
        IBaseRepository<User> baseUserRepository,
        IBaseRepository<Liability> baseLiabilityRepository,
        IBaseRepository<Person> basePersonRepository,
        IBaseRepository<Company> baseCompanyRepository)
    {
        _baseEventRepository = baseEventRepository;
        _baseEventFileRepository = baseEventFileRepository;
        _baseUserRepository = baseUserRepository;
        _baseLiabilityRepository = baseLiabilityRepository;
        _basePersonRepository = basePersonRepository;
        _baseCompanyRepository = baseCompanyRepository;
    }

    public async Task<PagedResult<EventResult>> GetListEventAsync(EventQuery eventQuery)
    {
        var query = _baseEventRepository.BuildQueryable(
            new List<string> 
            { 
                "AddedByUser", 
                "Liability", 
                "Person", 
                "Company", 
                "Files" 
            },
            null
        );

        if (!string.IsNullOrEmpty(eventQuery.Title))
        {
            var title = eventQuery.Title.ToLower();
            query = query.Where(e => e.Title.ToLower().Contains(title));
        }

        if (!string.IsNullOrEmpty(eventQuery.Type))
        {
            var type = eventQuery.Type.ToLower();
            query = query.Where(e => e.Type.ToLower().Contains(type));
        }

        if (eventQuery.DateFrom.HasValue)
        {
            query = query.Where(e => e.Date >= eventQuery.DateFrom.Value);
        }

        if (eventQuery.DateTo.HasValue)
        {
            query = query.Where(e => e.Date <= eventQuery.DateTo.Value);
        }

        if (eventQuery.IsSystem.HasValue)
        {
            query = query.Where(e => e.IsSystem == eventQuery.IsSystem.Value);
        }

        if (eventQuery.IsRepeating.HasValue)
        {
            query = query.Where(e => e.IsRepeating == eventQuery.IsRepeating.Value);
        }

        if (eventQuery.IsDismissed.HasValue)
        {
            query = query.Where(e => e.IsDismissed == eventQuery.IsDismissed.Value);
        }

        if (eventQuery.AddedByUserId.HasValue)
        {
            query = query.Where(e => e.AddedByUserId == eventQuery.AddedByUserId.Value);
        }

        if (eventQuery.LiabilityId.HasValue)
        {
            query = query.Where(e => e.LiabilityId == eventQuery.LiabilityId.Value);
        }

        if (eventQuery.PersonId.HasValue)
        {
            query = query.Where(e => e.PersonId == eventQuery.PersonId.Value);
        }

        if (eventQuery.CompanyId.HasValue)
        {
            query = query.Where(e => e.CompanyId == eventQuery.CompanyId.Value);
        }

        var totalCount = await query.CountAsync();

        if (string.IsNullOrEmpty(eventQuery.SortBy))
        {
            query = query.OrderByDescending(e => e.Date);
        }
        else
        {
            query = _baseEventRepository.ApplySorting(query, eventQuery.SortBy, eventQuery.SortDescending);
        }

        var results = await query
            .Skip((eventQuery.PageNumber - 1) * eventQuery.PageSize)
            .Take(eventQuery.PageSize)
            .Select(EventResult.FromEvent)
            .ToListAsync();

        return new PagedResult<EventResult>(results, totalCount, eventQuery.PageNumber, eventQuery.PageSize);
    }

    public async Task<EventResult> GetEventByIdAsync(Guid id)
    {
        var eventItem = await _baseEventRepository
            .BuildQueryable(
                new List<string> 
                { 
                    "AddedByUser", 
                    "Liability", 
                    "Person", 
                    "Company", 
                    "Files" 
                },
                e => e.Id == id
            )
            .Select(EventResult.FromEvent)
            .FirstOrDefaultAsync();

        if (eventItem == null)
        {
            throw new ResponseErrorObject("Event not found", StatusCodes.Status404NotFound);
        }

        return eventItem;
    }

    public async Task<EventResult> CreateEventAsync(CreateEventInput request)
    {
        var userExists = await _baseUserRepository.GetByIdAsync(request.AddedByUserId);
        if (userExists == null)
        {
            throw new ResponseErrorObject("User not found", StatusCodes.Status400BadRequest);
        }

        if (request.LiabilityId.HasValue)
        {
            var liabilityExists = await _baseLiabilityRepository.GetByIdAsync(request.LiabilityId.Value);
            if (liabilityExists == null)
            {
                throw new ResponseErrorObject("Liability not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.PersonId.HasValue)
        {
            var personExists = await _basePersonRepository.GetByIdAsync(request.PersonId.Value);
            if (personExists == null)
            {
                throw new ResponseErrorObject("Person not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.CompanyId.HasValue)
        {
            var companyExists = await _baseCompanyRepository.GetByIdAsync(request.CompanyId.Value);
            if (companyExists == null)
            {
                throw new ResponseErrorObject("Company not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.IsRepeating)
        {
            if (!request.RepeatNumber.HasValue || string.IsNullOrEmpty(request.RepeatUnit))
            {
                throw new ResponseErrorObject("Repeating events must have RepeatNumber and RepeatUnit", StatusCodes.Status400BadRequest);
            }
        }

        var eventItem = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Type = request.Type,
            Date = request.Date,
            IsSystem = request.IsSystem,
            IsRepeating = request.IsRepeating,
            RepeatNumber = request.RepeatNumber,
            RepeatUnit = request.RepeatUnit,
            IsDismissed = request.IsDismissed,
            RepeatingDateDismissed = request.RepeatingDateDismissed,
            ModifiedValuesJson = request.ModifiedValuesJson,
            ModifiedValuesObject = request.ModifiedValuesObject,
            AddedByUserId = request.AddedByUserId,
            LiabilityId = request.LiabilityId,
            PersonId = request.PersonId,
            CompanyId = request.CompanyId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _baseEventRepository.CreateAsync(eventItem);

        if (request.Files != null && request.Files.Any())
        {
            var files = request.Files.Select(f => new EventFile
            {
                Id = Guid.NewGuid(),
                EventId = eventItem.Id,
                Filename = f.Filename,
                Url = f.Url,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _baseEventFileRepository.CreateAsync(files);
        }

        return await GetEventByIdAsync(eventItem.Id);
    }

    public async Task<EventResult> UpdateEventAsync(Guid eventId, UpdateEventInput request)
    {
        var eventItem = await _baseEventRepository.GetByIdAsync(eventId);
        if (eventItem == null)
        {
            throw new ResponseErrorObject("Event not found", StatusCodes.Status404NotFound);
        }

        var userExists = await _baseUserRepository.GetByIdAsync(request.AddedByUserId);
        if (userExists == null)
        {
            throw new ResponseErrorObject("User not found", StatusCodes.Status400BadRequest);
        }

        if (request.LiabilityId.HasValue)
        {
            var liabilityExists = await _baseLiabilityRepository.GetByIdAsync(request.LiabilityId.Value);
            if (liabilityExists == null)
            {
                throw new ResponseErrorObject("Liability not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.PersonId.HasValue)
        {
            var personExists = await _basePersonRepository.GetByIdAsync(request.PersonId.Value);
            if (personExists == null)
            {
                throw new ResponseErrorObject("Person not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.CompanyId.HasValue)
        {
            var companyExists = await _baseCompanyRepository.GetByIdAsync(request.CompanyId.Value);
            if (companyExists == null)
            {
                throw new ResponseErrorObject("Company not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.IsRepeating)
        {
            if (!request.RepeatNumber.HasValue || string.IsNullOrEmpty(request.RepeatUnit))
            {
                throw new ResponseErrorObject("Repeating events must have RepeatNumber and RepeatUnit", StatusCodes.Status400BadRequest);
            }
        }

        eventItem.Title = request.Title;
        eventItem.Description = request.Description;
        eventItem.Type = request.Type;
        eventItem.Date = request.Date;
        eventItem.IsSystem = request.IsSystem;
        eventItem.IsRepeating = request.IsRepeating;
        eventItem.RepeatNumber = request.RepeatNumber;
        eventItem.RepeatUnit = request.RepeatUnit;
        eventItem.IsDismissed = request.IsDismissed;
        eventItem.RepeatingDateDismissed = request.RepeatingDateDismissed;
        eventItem.ModifiedValuesJson = request.ModifiedValuesJson;
        eventItem.ModifiedValuesObject = request.ModifiedValuesObject;
        eventItem.AddedByUserId = request.AddedByUserId;
        eventItem.LiabilityId = request.LiabilityId;
        eventItem.PersonId = request.PersonId;
        eventItem.CompanyId = request.CompanyId;
        eventItem.UpdatedAt = DateTime.UtcNow;

        await _baseEventRepository.UpdateAsync(eventItem);

        if (request.Files != null)
        {
            await _baseEventFileRepository.DeleteAsync(f => f.EventId == eventId);

            if (request.Files.Any())
            {
                var files = request.Files.Select(f => new EventFile
                {
                    Id = Guid.NewGuid(),
                    EventId = eventItem.Id,
                    Filename = f.Filename,
                    Url = f.Url,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _baseEventFileRepository.CreateAsync(files);
            }
        }

        return await GetEventByIdAsync(eventItem.Id);
    }

    public async Task DeleteEventAsync(Guid id)
    {
        var eventItem = await _baseEventRepository.GetByIdAsync(id);
        if (eventItem == null)
        {
            throw new ResponseErrorObject("Event not found", StatusCodes.Status404NotFound);
        }

        await _baseEventRepository.DeleteAsync(id);
    }

    public async Task<EventResult> DismissEventAsync(Guid id, DateTime? repeatingDateDismissed = null)
    {
        var eventItem = await _baseEventRepository.GetByIdAsync(id);
        if (eventItem == null)
        {
            throw new ResponseErrorObject("Event not found", StatusCodes.Status404NotFound);
        }

        eventItem.IsDismissed = true;
        eventItem.RepeatingDateDismissed = repeatingDateDismissed;
        eventItem.UpdatedAt = DateTime.UtcNow;

        await _baseEventRepository.UpdateAsync(eventItem);

        return await GetEventByIdAsync(eventItem.Id);
    }
}