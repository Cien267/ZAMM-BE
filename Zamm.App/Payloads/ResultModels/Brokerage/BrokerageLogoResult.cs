namespace Zamm.Application.Payloads.ResultModels.Brokerage;

public class BrokerageLogoResult
{
    public Guid Id { get; set; }
    public string Url { get; set; } = null!;
    public Guid BrokerageId { get; set; }
}