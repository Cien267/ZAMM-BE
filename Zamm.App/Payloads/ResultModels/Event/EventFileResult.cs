namespace Zamm.Application.Payloads.ResultModels.Event;

public class EventFileResult
{
    public Guid Id { get; set; }
    public string Filename { get; set; } = null!;
    public string Url { get; set; } = null!;
    public Guid EventId { get; set; }
    public DateTime CreatedAt { get; set; }
}