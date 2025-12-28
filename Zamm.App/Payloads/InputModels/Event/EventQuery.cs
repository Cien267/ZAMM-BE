using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Event;

public class EventQuery : PaginationParams
{
    public string? Title { get; set; }
    public string? Type { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool? IsSystem { get; set; }
    public bool? IsRepeating { get; set; }
    public bool? IsDismissed { get; set; }
    public Guid? AddedByUserId { get; set; }
    public Guid? LiabilityId { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? CompanyId { get; set; }
}