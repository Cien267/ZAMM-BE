using Zamm.Application.Payloads.InputModels.Event;
using Zamm.Application.Payloads.ResultModels.Event;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService;

public interface IEventService
{
    Task<PagedResult<EventResult>> GetListEventAsync(EventQuery query);
    Task<EventResult> GetEventByIdAsync(Guid id);
    Task<EventResult> CreateEventAsync(CreateEventInput request);
    Task<EventResult> UpdateEventAsync(Guid eventId, UpdateEventInput request);
    Task DeleteEventAsync(Guid id);
    Task<EventResult> DismissEventAsync(Guid id, DateTime? repeatingDateDismissed = null);
}