using Zamm.Application.Payloads.InputModels.Note;
using Zamm.Application.Payloads.ResultModels.Note;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface INoteService
{
    Task<PagedResult<NoteResult>> GetListNoteAsync(NoteQuery query);
    Task<NoteResult> GetNoteByIdAsync(Guid id);
    Task<NoteResult> CreateNoteAsync(CreateNoteInput request);
    Task<NoteResult> UpdateNoteAsync(Guid noteId, UpdateNoteInput request);
    Task DeleteNoteAsync(Guid id);
}