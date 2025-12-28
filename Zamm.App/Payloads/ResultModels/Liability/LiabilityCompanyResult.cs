namespace Zamm.Application.Payloads.ResultModels.Liability;

public class LiabilityCompanyResult
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public decimal Percent { get; set; }
}