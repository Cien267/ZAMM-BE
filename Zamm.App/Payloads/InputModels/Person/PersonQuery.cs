using Zamm.Application.Payloads.InputModels.Common;

namespace Zamm.Application.Payloads.InputModels.Person;

public class PersonQuery : PaginationParams
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? BrokerId { get; set; }
}