namespace Zamm.Application.Payloads.ResultModels.Liability;

public class LiabilityPersonResult
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = null!;
    public decimal Percent { get; set; }
}