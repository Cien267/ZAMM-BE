using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zamm.Application.InterfaceService;
using Zamm.Application.Payloads.InputModels.Event;
using Zamm.Application.Payloads.ResultModels.Event;
using Zamm.Shared.Models;

namespace Zamm.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/event")]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<EventResult>>> GetListEventAsync([FromQuery] EventQuery query)
    {
        var result = await _eventService.GetListEventAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EventResult>> GetEventByIdAsync(Guid id)
    {
        var result = await _eventService.GetEventByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<EventResult>> CreateEventAsync([FromBody] CreateEventInput request)
    {
        var result = await _eventService.CreateEventAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EventResult>> UpdateEventAsync(Guid id, [FromBody] UpdateEventInput request)
    {
        var result = await _eventService.UpdateEventAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEventAsync(Guid id)
    {
        await _eventService.DeleteEventAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/dismiss")]
    public async Task<ActionResult<EventResult>> DismissEventAsync(Guid id, [FromBody] DismissEventRequest? request = null)
    {
        var result = await _eventService.DismissEventAsync(id, request?.RepeatingDateDismissed);
        return Ok(result);
    }
}