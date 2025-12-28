using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Note;

public class NoteQuery : PaginationParams
{
    public string? Text { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid? LiabilityId { get; set; }
    public Guid? EventId { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? CompanyId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}