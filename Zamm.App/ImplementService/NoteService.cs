using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Note;
using Zamm.Application.Payloads.Responses;
using Zamm.Application.Payloads.ResultModels.Note;
using Zamm.Domain.Entities;
using Zamm.Domain.InterfaceRepositories;
using Zamm.Shared.Models;

namespace Zamm.Application.ImplementService;

public class NoteService : INoteService
{
    private readonly IBaseRepository<Note> _baseNoteRepository;
    private readonly IBaseRepository<User> _baseUserRepository;
    private readonly IBaseRepository<Liability> _baseLiabilityRepository;
    private readonly IBaseRepository<Event> _baseEventRepository;
    private readonly IBaseRepository<Person> _basePersonRepository;
    private readonly IBaseRepository<Company> _baseCompanyRepository;

    public NoteService(
        IBaseRepository<Note> baseNoteRepository,
        IBaseRepository<User> baseUserRepository,
        IBaseRepository<Liability> baseLiabilityRepository,
        IBaseRepository<Event> baseEventRepository,
        IBaseRepository<Person> basePersonRepository,
        IBaseRepository<Company> baseCompanyRepository)
    {
        _baseNoteRepository = baseNoteRepository;
        _baseUserRepository = baseUserRepository;
        _baseLiabilityRepository = baseLiabilityRepository;
        _baseEventRepository = baseEventRepository;
        _basePersonRepository = basePersonRepository;
        _baseCompanyRepository = baseCompanyRepository;
    }

    public async Task<PagedResult<NoteResult>> GetListNoteAsync(NoteQuery noteQuery)
    {
        var query = _baseNoteRepository.BuildQueryable(
            new List<string> 
            { 
                "Author", 
                "EditedBy", 
                "Liability", 
                "Event", 
                "Person", 
                "Company" 
            },
            null
        );

        if (!string.IsNullOrEmpty(noteQuery.Text))
        {
            var text = noteQuery.Text.ToLower();
            query = query.Where(n => n.Text.ToLower().Contains(text));
        }

        if (noteQuery.AuthorId.HasValue)
        {
            query = query.Where(n => n.AuthorId == noteQuery.AuthorId.Value);
        }

        if (noteQuery.LiabilityId.HasValue)
        {
            query = query.Where(n => n.LiabilityId == noteQuery.LiabilityId.Value);
        }

        if (noteQuery.EventId.HasValue)
        {
            query = query.Where(n => n.EventId == noteQuery.EventId.Value);
        }

        if (noteQuery.PersonId.HasValue)
        {
            query = query.Where(n => n.PersonId == noteQuery.PersonId.Value);
        }

        if (noteQuery.CompanyId.HasValue)
        {
            query = query.Where(n => n.CompanyId == noteQuery.CompanyId.Value);
        }

        if (noteQuery.CreatedFrom.HasValue)
        {
            query = query.Where(n => n.CreatedAt >= noteQuery.CreatedFrom.Value);
        }

        if (noteQuery.CreatedTo.HasValue)
        {
            query = query.Where(n => n.CreatedAt <= noteQuery.CreatedTo.Value);
        }

        var totalCount = await query.CountAsync();

        if (string.IsNullOrEmpty(noteQuery.SortBy))
        {
            query = query.OrderByDescending(n => n.CreatedAt);
        }
        else
        {
            query = _baseNoteRepository.ApplySorting(query, noteQuery.SortBy, noteQuery.SortDescending);
        }

        var results = await query
            .Skip((noteQuery.PageNumber - 1) * noteQuery.PageSize)
            .Take(noteQuery.PageSize)
            .Select(NoteResult.FromNote)
            .ToListAsync();

        return new PagedResult<NoteResult>(results, totalCount, noteQuery.PageNumber, noteQuery.PageSize);
    }

    public async Task<NoteResult> GetNoteByIdAsync(Guid id)
    {
        var note = await _baseNoteRepository
            .BuildQueryable(
                new List<string> 
                { 
                    "Author", 
                    "EditedBy", 
                    "Liability", 
                    "Event", 
                    "Person", 
                    "Company" 
                },
                n => n.Id == id
            )
            .Select(NoteResult.FromNote)
            .FirstOrDefaultAsync();

        if (note == null)
        {
            throw new ResponseErrorObject("Note not found", StatusCodes.Status404NotFound);
        }

        return note;
    }

    public async Task<NoteResult> CreateNoteAsync(CreateNoteInput request)
    {
        var authorExists = await _baseUserRepository.GetByIdAsync(request.AuthorId);
        if (authorExists == null)
        {
            throw new ResponseErrorObject("Author not found", StatusCodes.Status400BadRequest);
        }

        if (!request.LiabilityId.HasValue && 
            !request.EventId.HasValue && 
            !request.PersonId.HasValue && 
            !request.CompanyId.HasValue)
        {
            throw new ResponseErrorObject("Note must be linked to at least one entity (Liability, Event, Person, or Company)", StatusCodes.Status400BadRequest);
        }

        if (request.LiabilityId.HasValue)
        {
            var liabilityExists = await _baseLiabilityRepository.GetByIdAsync(request.LiabilityId.Value);
            if (liabilityExists == null)
            {
                throw new ResponseErrorObject("Liability not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.EventId.HasValue)
        {
            var eventExists = await _baseEventRepository.GetByIdAsync(request.EventId.Value);
            if (eventExists == null)
            {
                throw new ResponseErrorObject("Event not found", StatusCodes.Status400BadRequest);
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

        var note = new Note
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            AuthorId = request.AuthorId,
            LiabilityId = request.LiabilityId,
            EventId = request.EventId,
            PersonId = request.PersonId,
            CompanyId = request.CompanyId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _baseNoteRepository.CreateAsync(note);

        return await GetNoteByIdAsync(note.Id);
    }

    public async Task<NoteResult> UpdateNoteAsync(Guid noteId, UpdateNoteInput request)
    {
        var note = await _baseNoteRepository.GetByIdAsync(noteId);
        if (note == null)
        {
            throw new ResponseErrorObject("Note not found", StatusCodes.Status404NotFound);
        }

        var editorExists = await _baseUserRepository.GetByIdAsync(request.EditedById);
        if (editorExists == null)
        {
            throw new ResponseErrorObject("Editor not found", StatusCodes.Status400BadRequest);
        }

        if (!request.LiabilityId.HasValue && 
            !request.EventId.HasValue && 
            !request.PersonId.HasValue && 
            !request.CompanyId.HasValue)
        {
            throw new ResponseErrorObject("Note must be linked to at least one entity (Liability, Event, Person, or Company)", StatusCodes.Status400BadRequest);
        }

        if (request.LiabilityId.HasValue)
        {
            var liabilityExists = await _baseLiabilityRepository.GetByIdAsync(request.LiabilityId.Value);
            if (liabilityExists == null)
            {
                throw new ResponseErrorObject("Liability not found", StatusCodes.Status400BadRequest);
            }
        }

        if (request.EventId.HasValue)
        {
            var eventExists = await _baseEventRepository.GetByIdAsync(request.EventId.Value);
            if (eventExists == null)
            {
                throw new ResponseErrorObject("Event not found", StatusCodes.Status400BadRequest);
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

        note.Text = request.Text;
        note.EditedById = request.EditedById;
        note.LiabilityId = request.LiabilityId;
        note.EventId = request.EventId;
        note.PersonId = request.PersonId;
        note.CompanyId = request.CompanyId;
        note.UpdatedAt = DateTime.UtcNow;

        await _baseNoteRepository.UpdateAsync(note);

        return await GetNoteByIdAsync(note.Id);
    }

    public async Task DeleteNoteAsync(Guid id)
    {
        var note = await _baseNoteRepository.GetByIdAsync(id);
        if (note == null)
        {
            throw new ResponseErrorObject("Note not found", StatusCodes.Status404NotFound);
        }

        await _baseNoteRepository.DeleteAsync(id);
    }
}