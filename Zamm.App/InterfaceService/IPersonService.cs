using Zamm.Application.Payloads.InputModels.Person;
using Zamm.Application.Payloads.ResultModels.Person;
using Zamm.Shared.Models;

namespace Zamm.Application.InterfaceService
{
    public interface IPersonService
    {
        Task<PagedResult<PersonResult>> GetListPeopleAsync(PersonQuery query);
        Task<PersonResult> GetPersonByIdAsync(Guid id);
        Task<PersonResult> CreatePersonAsync(CreatePersonInput request);
        Task<PersonResult> UpdatePersonAsync(Guid personId, UpdatePersonInput request);
        Task DeletePersonAsync(Guid id);
    }
}
