namespace Zamm.Application.Payloads.ResultModels.Liability;

public class FixedRatePeriodResult
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public int Term { get; set; }
    public decimal? CustomRate { get; set; }
}