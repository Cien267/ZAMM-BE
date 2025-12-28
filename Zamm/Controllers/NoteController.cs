using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Note;
using Zamm.Application.Payloads.ResultModels.Note;
using Zamm.Shared.Models;

namespace Zamm.Controllers;

[ApiController]
[Route("api/note")]
public class NoteController : ControllerBase
{
    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<NoteResult>>> GetListNoteAsync([FromQuery] NoteQuery query)
    {
        var result = await _noteService.GetListNoteAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NoteResult>> GetNoteByIdAsync(Guid id)
    {
        var result = await _noteService.GetNoteByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<NoteResult>> CreateNoteAsync([FromBody] CreateNoteInput request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _noteService.CreateNoteAsync(request);
        return CreatedAtAction(nameof(GetNoteByIdAsync), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<NoteResult>> UpdateNoteAsync(Guid id, [FromBody] UpdateNoteInput request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _noteService.UpdateNoteAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNoteAsync(Guid id)
    {
        await _noteService.DeleteNoteAsync(id);
        return NoContent();
    }
}