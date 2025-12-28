namespace Zamm.Application.Payloads.ResultModels.Company;

public class CompanyPersonResult
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = null!;
}